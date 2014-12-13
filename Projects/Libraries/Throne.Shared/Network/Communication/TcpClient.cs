using System;
using System.Net;
using System.Net.Sockets;
using Throne.Shared.Network.Connectivity;
using Throne.Shared.Security.Permissions;
using Throne.Shared.Threading.Actors;

namespace Throne.Shared.Network.Communication
{
    public abstract class TcpClient : Actor<TcpClient>
    {
        protected const Int32 RcvBufferLength = UInt16.MaxValue;

        protected Byte[] rcvBuffer;

        protected readonly Object sendLock;

        protected Socket socket;

        protected Boolean disconnected;
        protected SocketAsyncEventArgs rcvSaeArgs;

        protected TcpClient(ConnectionEventArgs args)
        {
            socket = args.Sock;
            socket.NoDelay = socket.DontFragment = true;

            sendLock = new Object();
            rcvBuffer = new Byte[RcvBufferLength];

            BeginReceive();
        }

        ~TcpClient()
        {
            Console.Write("TcpClient disposed");
        }

        public IPAddress IP
        {
            get
            {
                var ipEndPoint = socket.RemoteEndPoint as IPEndPoint;
                return ipEndPoint != null ? ipEndPoint.Address : null;
            }
        }

        public Int32 SocketId
        {
            get { return socket.GetHashCode(); }
        }

        protected Int32 BytesTransferred
        {
            get { return rcvSaeArgs.BytesTransferred; }
        }

        protected Boolean Connected
        {
            get { return socket.Connected && !disconnected; }
        }

        protected virtual void BeginReceive()
        {
            rcvSaeArgs = SocketAsyncEventArgsPool.Acquire(IOComplete);
            rcvSaeArgs.SetBuffer(rcvBuffer, 0, RcvBufferLength);
            Receive();
        }

        protected void Receive()
        {
            if (!Connected)
            {
                if (!disconnected) Disconnect();
            }
            else
            {
                if (!socket.ReceiveAsync(rcvSaeArgs))
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
            if (!Connected && io.SocketError != SocketError.Success && !disconnected)
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
            rcvSaeArgs.Completed -= IOComplete;

            socket.Terminate();

            disconnected = true;
        }

        protected override void Dispose(bool disposing)
        {
            rcvSaeArgs.Release();
            base.Dispose(disposing);
        }
    }
}