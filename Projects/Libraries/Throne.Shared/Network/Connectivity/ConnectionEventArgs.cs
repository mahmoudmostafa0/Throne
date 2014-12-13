using System;
using System.Net.Sockets;
using Throne.Shared.Network.Communication;
using Throne.Shared.Network.Handling;
using Throne.Shared.Network.Security;

namespace Throne.Shared.Network.Connectivity
{
    public sealed class ConnectionEventArgs : EventArgs
    {
        public ConnectionEventArgs(Socket sock, IPacketPropagator propagator,
            INetworkCipher cipher = null)
        {
            Sock = sock;
            PacketPropagator = propagator;
            StreamCipher = cipher;
        }

        public ConnectionEventArgs(IClient client)
        {
            Client = client;
        }

        public IClient Client { get; private set; }
        public Socket Sock { get; private set; }
        public IPacketPropagator PacketPropagator { get; private set; }
        public INetworkCipher StreamCipher { get; set; }
    }
}