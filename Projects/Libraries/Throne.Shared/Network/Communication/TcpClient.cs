using System;
using System.Net;
using System.Net.Sockets;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Security.Permissions;
using Throne.Framework.Threading.Actors;

namespace Throne.Framework.Network.Communication
{
    public abstract class TcpClient : Actor<TcpClient>
    {
        protected const Int32 RcvBufferLength = UInt16.MaxValue;

        protected Boolean Disconnected;
        protected Byte[] rcvBuffer;
        protected SocketAsyncEventArgs RcvSaeArgs;
        protected readonly Socket Socket;

        protected TcpClient(ConnectionEventArgs args)
        {
            Socket = args.Sock;
            Socket.NoDelay = Socket.DontFragment = true;

            rcvBuffer = new Byte[RcvBufferLength];

            BeginReceive();
        }

        public IPAddress IP
        {
            get
            {
                var ipEndPoint = Socket.RemoteEndPoint as IPEndPoint;
                return ipEndPoint != null ? ipEndPoint.Address : null;
            }
        }

        public Int32 SocketId
        {
            get { return Socket.GetHashCode(); }
        }

        protected Int32 BytesTransferred
        {
            get { return RcvSaeArgs.BytesTransferred; }
        }

        protected Boolean Connected
        {
            get { return Socket.Connected && !Disconnected; }
        }

        ~TcpClient()
        {
            Console.Write("TcpClient disposed");
        }

        protected virtual void BeginReceive()
        {
            RcvSaeArgs = SocketAsyncEventArgsPool.Acquire(IOComplete);
            RcvSaeArgs.SetBuffer(rcvBuffer, 0, RcvBufferLength);
            Receive();
        }

        protected void Receive()
        {
            if (!Connected)
            {
                if (!Disconnected) Disconnect();
            }
            else
            {
                if (!Socket.ReceiveAsync(RcvSaeArgs))
                    OnReceive();
            }
        }

        protected abstract void OnReceive();

        public abstract void Send(byte[] value);

        protected void OnSend(SocketAsyncEventArgs sndSocketArgs)
        {
            sndSocketArgs.Completed -= IOComplete;
            sndSocketArgs.Release();
        }

        protected void IOComplete(object caller, SocketAsyncEventArgs io)
        {
            if (!Connected && io.SocketError != SocketError.Success && !Disconnected)
                Disconnect();
            else
                switch (io.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        OnReceive();
                        break;
                    case SocketAsyncOperation.Disconnect:
                        Disconnect();
                        break;
                    case SocketAsyncOperation.Send:
                        OnSend(io);
                        break;
                }
        }

        public virtual void Disconnect()
        {
            if (!HasPermission(typeof (ConnectedPermission))) return;

            RemovePermission(typeof (ConnectedPermission));
            RcvSaeArgs.Completed -= IOComplete;

            Socket.Terminate();

            Disconnected = true;
        }

        protected override void Dispose(bool disposing)
        {
            RcvSaeArgs.Release();
            base.Dispose(disposing);
        }

        public static implicit operator Boolean(TcpClient tcpClient)
        {
            return tcpClient != null;
        }
    }
}