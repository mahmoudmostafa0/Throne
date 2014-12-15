using System;
using System.Net;
using System.Net.Sockets;
using Throne.Shared;
using Throne.Shared.Exceptions;
using Throne.Shared.Logging;
using Throne.Shared.Network.Communication;
using Throne.Shared.Network.Connectivity;
using Throne.Shared.Network.Handling;
using Throne.Shared.Network.Security;
using TcpClient = Throne.Shared.Network.Communication.TcpClient;

#pragma warning disable 618

namespace Throne.Login.Network
{
    public sealed class AuthenticationClient : TcpClient, IClient
    {
        private const Int32 MIN_MSG_SIZE = 4;
        private const Int32 MAX_MSG_SIZE = 1016;
        private const Int32 MAX_STREAM_SIZE = UInt16.MaxValue;

        private readonly LogProxy _log = new LogProxy("AuthClient");


        private readonly Int32 _minStreamSize;

        private readonly IPacketPropagator _propagator;

        public AuthenticationClient(ConnectionEventArgs args)
            : base(args)
        {
            _minStreamSize = TcpServer.IncomingFooterLength + MIN_MSG_SIZE;
            _propagator = args.PacketPropagator;
            StreamCipher = args.StreamCipher;

            Log.Info("{0} connected", ClientAddress);
        }

        public override void Send(byte[] value)
        {
            if (Connected)
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
                catch (Exception ex)
                {
                    Disconnect();

                    if (ex is ObjectDisposedException)
                        return;

                    ExceptionManager.RegisterException(ex);
                }
            }
            else if (!disconnected) Disconnect();
        }

        public IPAddress ClientAddress
        {
            get { return ((IPEndPoint)socket.RemoteEndPoint).Address; }
        }

        /// <summary>
        ///     This dynamic variable holds authentication data for login, then the account once verified and loaded.
        /// </summary>
        public dynamic UserData { get; set; }

        public void DisconnectWithMessage(Byte[] message)
        {
            throw new NotImplementedException();
        }

        public INetworkCipher StreamCipher { get; private set; }

        public INetworkCipher TransferCipher { get; set; }

        public LogProxy Log
        {
            get { return _log; }
        }

        protected override unsafe void OnReceive()
        {
            if (BytesTransferred >= _minStreamSize)
            {
                var decipheredBuffer = StreamCipher.Decrypt(rcvBuffer, BytesTransferred);
                var position = new Int32();

                while (position < BytesTransferred && !disconnected)
                    fixed (Byte* src = decipheredBuffer)
                    {
                        byte* srcP = src + position;
                        short msgSize = *(Int16*)srcP;
                        short msgType = *(Int16*)(srcP + 2);

                        if (!msgSize.IsBetween<Int16>(MIN_MSG_SIZE, MAX_MSG_SIZE))
                            break;

                        var msg = new Byte[msgSize];

                        fixed (Byte* dst = msg)
                            for (byte* dstP = dst;
                                msgSize-- > 0 && position < BytesTransferred;
                                position++)
                                *dstP++ = *srcP++;

                        _propagator.Handle(this, msgType, msg, (Int16)msg.Length);
                    }
            }

            if (BytesTransferred < _minStreamSize)
            {
                Log.Error("Connection dropped because of empty stream. {0}", ClientAddress);
                Disconnect();
            }
            else
                Receive();
        }
    }
}

#pragma warning restore 618