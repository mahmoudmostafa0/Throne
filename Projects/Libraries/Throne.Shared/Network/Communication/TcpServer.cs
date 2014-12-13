using System;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;
using Throne.Shared.Exceptions;
using Throne.Shared.Network.Connectivity;
using Throne.Shared.Network.Handling;
using Throne.Shared.Network.Security.Firewall;

namespace Throne.Shared.Network.Communication
{
    public sealed class TcpServer : Socket
    {
        private readonly IPacketPropagator _propagator;

        public TcpServer(IPacketPropagator packetPropagator)
            : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            _propagator = packetPropagator;
        }

        #region Server Information

        /// <summary>
        ///     The name of the server.
        /// </summary>
        public String Name { [UsedImplicitly] get; private set; }

        /// <summary>
        ///     The text for each outgoing packet footer.
        /// </summary>
        public static String OutgoingFooter { get; private set; }

        /// <summary>
        ///     Footer length for outgoing packets.
        /// </summary>
        public static Int16 OutgoingFooterLength { get; private set; }

        /// <summary>
        ///     The text for each incoming packet footer.
        /// </summary>
        public static String IncomingFooter { get; private set; }

        /// <summary>
        ///     Footer length for incoming packets.
        /// </summary>
        public static Int16 IncomingFooterLength { get; private set; }

        #endregion

        /// <summary>
        ///     An interface to Windows Firewall, will protect the server by adding rules
        ///     to the hnetcfg.fwpolicy2. As such, it blocks server ports from malicious IP addresses.
        /// </summary>
        private NetFwHandler NetFw { get; set; }

        public static event EventHandler<ConnectionEventArgs> ClientConnected;
        public static event EventHandler<ConnectionEventArgs> ClientDisconnected;

        public void Start(
            String name,
            String outgoingFooter,
            String incomingFooter,
            IPEndPoint ipEndPoint,
            Int32 backlog,
            Int32 fwWatchSeconds = 10,
            Int32 fwConnectionThreshold = 10,
            Boolean noNagle = true,
            Boolean keepAlive = true,
            Boolean gracefulShutdown = true,
            Boolean dontFragment = true,
            Boolean reuseIpEndPoint = true)
        {
            Name = name;
            OutgoingFooter = outgoingFooter;
            OutgoingFooterLength = (Int16) outgoingFooter.Length;
            IncomingFooter = incomingFooter;
            IncomingFooterLength = (Int16) incomingFooter.Length;
            EndPoint = ipEndPoint;

            NetFw = new NetFwHandler(fwWatchSeconds, fwConnectionThreshold);

            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment, dontFragment);
            SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, noNagle);
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, keepAlive);
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, gracefulShutdown);
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseIpEndPoint);

            Bind(EndPoint);
            Listen(backlog);

            Accept(SocketAsyncEventArgsPool.Acquire(OnAccept));
        }

        public void Stop()
        {
            Shutdown(SocketShutdown.Both);
            Close();
        }


        public IPEndPoint EndPoint { get; private set; }

        private void Accept(SocketAsyncEventArgs accEa)
        {
            try
            {
                accEa.AcceptSocket = null;
                bool Event = AcceptAsync(accEa);
                if (!Event)
                    OnAccept(null, accEa);
            }

            catch (Exception e)
            {
                SocketAsyncEventArgs oldEa = accEa;
                oldEa.Completed -= OnAccept;
                Accept(SocketAsyncEventArgsPool.Acquire(OnAccept));
                oldEa.Release();
                ExceptionManager.RegisterException(e);
            }
        }

        private void OnAccept(object o, SocketAsyncEventArgs a)
        {
            Socket sock = a.AcceptSocket;
            if (!sock.Connected) return;

            try
            {
                if (!NetFw.PassFwPolicy(((IPEndPoint) sock.RemoteEndPoint).Address))
                {
                    NetFw.AddRule(((IPEndPoint) sock.RemoteEndPoint).Address, ((IPEndPoint) LocalEndPoint).Port);
                    sock.Terminate();
                    return;
                }
                
                var passConnectEvent = ClientConnected;
                if (passConnectEvent != null)
                    passConnectEvent(this, new ConnectionEventArgs(sock, _propagator));
            }
            catch (Exception ex)
            {
                ExceptionManager.RegisterException(ex);
            }
            Accept(a);
        }
    }
}