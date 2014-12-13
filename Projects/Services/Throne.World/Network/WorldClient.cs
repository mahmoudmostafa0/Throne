using System;
using System.Net;
using System.Net.Sockets;
using Throne.Shared;
using Throne.Shared.Commands;
using Throne.Shared.Cryptography;
using Throne.Shared.Exceptions;
using Throne.Shared.Logging;
using Throne.Shared.Math;
using Throne.Shared.Network.Communication;
using Throne.Shared.Network.Connectivity;
using Throne.Shared.Network.Handling;
using Throne.Shared.Network.Security;
using Throne.Shared.Threading.Actors;
using Throne.World.Network.Exchange;
using Throne.World.Network.Messages;
using Throne.World.Structures.Objects;
using TcpClient = Throne.Shared.Network.Communication.TcpClient;

namespace Throne.World.Network
{
    public sealed class WorldClient : TcpClient, IClient, ICommandUser
    {
        private const Int32 MIN_PACKET_SIZE = 4; //length+id
        private const Int32 MAX_PACKET_SIZE = 1016;
        private const Int32 MAX_STREAM_SIZE = UInt16.MaxValue;

        private readonly Int32 _minStreamSize;

        private ActorTimer _updateTimer;

        private readonly IPacketPropagator _propagator;
        private LogProxy _log = new LogProxy("WorldClient");

        public WorldClient(ConnectionEventArgs args)
            : base(args)
        {
            _minStreamSize = TcpServer.IncomingFooterLength + MIN_PACKET_SIZE;
            _propagator = args.PacketPropagator;
            StreamCipher = args.StreamCipher;
            UserData = new NetDragonDHKeyExchange();

            PostAsync(
                () =>
                {
                    using (var packet = new DiffieHellmanExchange(GameCipher.encryptionIv, GameCipher.decryptionIv))
                        Send(packet);
                });
        }

        ~WorldClient()
        {
            _log.Info(" Client disposed");
        }

        /// <summary>
        /// For sending responses to command usage.
        /// </summary>
        /// <param name="response"></param>
        void ICommandUser.Respond(string response)
        {
            Send(response);
        }

        public Character Character { get; private set; }

        /// <summary>
        ///     Sends given bytes as complete packets to the client.
        /// </summary>
        /// <param name="value"></param>
        public override void Send(byte[] value)
        {
            if (Connected)
            {
                lock (sendLock)
                {
                    SocketAsyncEventArgs sndSocketArgs = SocketAsyncEventArgsPool.Acquire(IOComplete);

                    if (StreamCipher != null)
                        value = StreamCipher.Encrypt(value, value.Length);

                    sndSocketArgs.SetBuffer(value, 0, value.Length);

                    try
                    {
                        if (!socket.SendAsync(sndSocketArgs))
                            OnSend(sndSocketArgs);
                    }
                    catch (Exception e)
                    {
                        Disconnect();

                        if (e is ObjectDisposedException)
                            return;

                        ExceptionManager.RegisterException(e);
                    }
                }
            }
            else if (!disconnected)
                Disconnect();
        }

        public IPAddress ClientAddress
        {
            get
            {
                return socket.Connected ? ((IPEndPoint)socket.RemoteEndPoint).Address : IPAddress.None;
            }
        }

        /// <value> This dynamic accessor holds the exchange data at connect then account data after authentication. </value>
        public dynamic UserData { get; set; }

        public void DisconnectWithMessage(Byte[] message)
        {
            Send(message);
            Disconnect();
        }

        /// <summary>
        ///     Set DH key to this cipher.
        /// </summary>
        public INetworkCipher StreamCipher { get; private set; }

        public INetworkCipher TransferCipher { get; set; }

        public LogProxy Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }

        public void Send(String msg)
        {
            Send(new ChatMessage(MessageChannel.Talk, msg));
        }

        protected override void BeginReceive()
        {
            rcvSaeArgs = SocketAsyncEventArgsPool.Acquire(OnExchange);
            rcvSaeArgs.SetBuffer(rcvBuffer, 0, RcvBufferLength);
            Receive();
        }

        private unsafe void OnExchange(object called, SocketAsyncEventArgs args)
        {
            rcvSaeArgs.Completed -= OnExchange;
            rcvSaeArgs.Completed += IOComplete;

            if (BytesTransferred > _minStreamSize && Connected && args.SocketError == SocketError.Success &&
                !disconnected)
            {
                var GameCipher = StreamCipher as GameCipher;
                if (GameCipher != null)
                {
                    //GameCipher.throneGameCipher.Initialize(RcvBuffer[0]);
                    GameCipher.Decrypt(rcvBuffer, null, BytesTransferred, 1);

                    //Thanks to Spirited Fang for the proper method to handle NetDragon's Diffie-Hellman response.
                    fixed (byte* packet = rcvBuffer)
                    {
                        int position = 7;
                        int packetLength = *(int*)(packet + position);
                        position += 4;
                        if (packetLength + 7 == BytesTransferred)
                        {
                            int junkLength = *(int*)(packet + position);
                            position += 4 + junkLength;
                            int responseLength = *(int*)(packet + position);
                            position += 4;
                            var dhresponse = new BigInteger(new string((sbyte*)packet, position, responseLength));
                            dynamic dhsecret = UserData.HandleResponse(dhresponse);
                            dynamic c5Key = NetDragonDHKeyExchange.ProcessDHSecret(dhsecret);

                            GameCipher.SetKey(c5Key);
                            Receive();
                            return;
                        }
                    }
                }
            }

            Log.Warn("{0} failed exchange. -Disconnected", ClientAddress);
            Disconnect();
        }

        protected override unsafe void OnReceive()
        {
            if (BytesTransferred >= _minStreamSize)
            {
                byte[] decipheredBuffer = StreamCipher.Decrypt(rcvBuffer, BytesTransferred);
                var position = new Int32();

                while (position < BytesTransferred && !disconnected)
                    fixed (Byte* src = decipheredBuffer)
                    {
                        byte* srcP = src + position;
                        short msgSize = *(Int16*)srcP;
                        short msgType = *(Int16*)(srcP + 2);

                        if (!msgSize.IsBetween<Int16>(MIN_PACKET_SIZE, MAX_PACKET_SIZE))
                        {
                            Disconnect();
                            break;
                        }

                        msgSize += TcpServer.IncomingFooterLength;

                        var msg = new Byte[msgSize];

                        fixed (Byte* dst = msg)
                            for (byte* dstP = dst;
                                 msgSize-- > 0 && position < BytesTransferred;
                                 position++)
                                *dstP++ = *srcP++;

                        _propagator.Handle(this, msgType, msg, (short)msg.Length);
                    }
            }

            if (BytesTransferred < _minStreamSize)
                Disconnect();
            else
                Receive();
        }

        public void SetCharacter(Character chr)
        {
            Character = chr;
        }

        public override void Disconnect()
        {
            Dispose();

            WorldServer.Instance.Info.OnlineCount--;
            WorldServer.Instance.Info.Update();
            base.Disconnect();
        }

        public override string ToString()
        {
            string name = Character ? " Name: {0}".Interpolate(Character.Name) : String.Empty;
            return "({0} IP: {1} )".Interpolate(name,
                ClientAddress ?? IPAddress.None);
        }

        protected override void Dispose(bool disposing)
        {
            if (Character)
            {
                Character.Save();
                Character.Dispose();
                SetCharacter(null);
            }
            base.Dispose(disposing);
        }
    }
}