using System;
using System.Net;
using System.Net.Sockets;
using Throne.Framework;
using Throne.Framework.Commands;
using Throne.Framework.Cryptography;
using Throne.Framework.Exceptions;
using Throne.Framework.Logging;
using Throne.Framework.Math;
using Throne.Framework.Network.Communication;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Handling;
using Throne.Framework.Network.Security;
using Throne.Framework.Threading.Actors;
using Throne.World.Network.Exchange;
using Throne.World.Network.Messages;
using Throne.World.Records;
using Throne.World.Structures.Objects;
using TcpClient = Throne.Framework.Network.Communication.TcpClient;

namespace Throne.World.Network
{
    public sealed class WorldClient : TcpClient, IClient
    {
        private const Int32 MIN_PACKET_SIZE = 4; //length+id
        private const Int32 MAX_PACKET_SIZE = 1016;
        private const Int32 MAX_STREAM_SIZE = UInt16.MaxValue;

        private readonly Int32 _minStreamSize;

        private readonly IPacketPropagator _propagator;
        private LogProxy _log = new LogProxy("WorldClient");
        private ActorTimer _updateTimer;

        public WorldClient(ConnectionEventArgs args)
            : base(args)
        {
            _minStreamSize = TcpServer.IncomingFooterLength + MIN_PACKET_SIZE;
            _propagator = args.PacketPropagator;
            StreamCipher = args.StreamCipher;
            ExchangeData = new NetDragonDHKeyExchange();

            PostAsync(
                () =>
                {
                    using (var packet = new DiffieHellmanExchange(GameCipher.encryptionIv, GameCipher.decryptionIv))
                        Send(packet);
                });
        }


        public Character Character { get; private set; }
        public NetDragonDHKeyExchange ExchangeData { get; set; }
        public AccountRecord AccountData { get; set; }

        public IPAddress ClientAddress
        {
            get { return Disconnected ? AccountData.IP : ((IPEndPoint) Socket.RemoteEndPoint).Address; }
        }

        public INetworkCipher StreamCipher { get; private set; }

        public INetworkCipher TransferCipher { get; set; }

        public LogProxy Log
        {
            get { return _log; }
            set { _log = value; }
        }

        #region Send and Receive

        /// <summary>
        ///     For sending responses to command usage.
        /// </summary>
        /// <param name="response"></param>
        void ICommandUser.Respond(String response)
        {
            Send(response);
        }

        /// <summary>
        ///     For sending responses to command usage.
        /// </summary>
        /// <param name="response"></param>
        void ICommandUser.Respond(Byte[] response)
        {
            Send(response);
        }

        /// <summary>
        ///     Sends given bytes as complete packets to the client.
        /// </summary>
        /// <param name="value"></param>
        public override void Send(Byte[] value)
        {
            if (Connected)
            {
                SocketAsyncEventArgs sndSocketArgs = SocketAsyncEventArgsPool.Acquire(IOComplete);

                if (StreamCipher != null)
                    value = StreamCipher.Encrypt(value, value.Length);

                sndSocketArgs.SetBuffer(value, 0, value.Length);

                try
                {
                    if (!Socket.SendAsync(sndSocketArgs))
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
            else if (!Disconnected)
                Disconnect();
        }

        public void SendArrays(params Byte[][] arrays)
        {
            foreach (var array in arrays)
                Send(array);
        }

        public void Send(String msg)
        {
            Send(new ChatMessage(MessageChannel.Talk, msg));
        }

        protected override void BeginReceive()
        {
            RcvSaeArgs = SocketAsyncEventArgsPool.Acquire(OnExchange);
            RcvSaeArgs.SetBuffer(rcvBuffer, 0, RcvBufferLength);
            Receive();
        }

        private unsafe void OnExchange(object called, SocketAsyncEventArgs args)
        {
            RcvSaeArgs.Completed -= OnExchange;
            RcvSaeArgs.Completed += IOComplete;

            if (BytesTransferred > _minStreamSize && Connected && args.SocketError == SocketError.Success &&
                !Disconnected)
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
                        int packetLength = *(int*) (packet + position);
                        position += 4;
                        if (packetLength + 7 == BytesTransferred)
                        {
                            int junkLength = *(int*) (packet + position);
                            position += 4 + junkLength;
                            int responseLength = *(int*) (packet + position);
                            position += 4;
                            var dhResponse = new BigInteger(new string((sbyte*) packet, position, responseLength));
                            byte[] dhSecret = ExchangeData.HandleResponse(dhResponse);
                            byte[] c5Key = NetDragonDHKeyExchange.ProcessDHSecret(dhSecret);

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

                while (position < BytesTransferred && !Disconnected)
                    fixed (Byte* src = decipheredBuffer)
                    {
                        byte* srcP = src + position;
                        short msgSize = *(Int16*) srcP;
                        short msgType = *(Int16*) (srcP + 2);

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

                        _propagator.Handle(this, msgType, msg, (short) msg.Length);
                    }
            }

            if (BytesTransferred < _minStreamSize)
                Disconnect();
            else
                Receive();
        }

        #endregion

        #region User Functions

        public override void Disconnect()
        {
            Dispose();

            WorldServer.Instance.Info.OnlineCount--;
            WorldServer.Instance.AccountService.Call(s => s.SetOnline(AccountData.Guid, false));

            base.Disconnect();
        }

        public void DisconnectWithMessage(Byte[] message)
        {
            Send(message);
            Disconnect();
        }

        public void SetCharacter(Character chr)
        {
            Character = chr;
        }

        public override string ToString()
        {
            return Character ? " Name: {0}".Interpolate(Character.Name) : String.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            if (Character)
            {
                Character.LoggedIn = false;
                Character.Save();
                Character.Dispose();
                SetCharacter(null);
            }
            base.Dispose(disposing);
        }

        #endregion

        ~WorldClient()
        {
            _log.Info(" Client disposed");
        }
    }
}