using System;
using Throne.Shared.Network.Connectivity;

namespace Throne.Shared.Network.Transmission
{
    public interface IPacketProcessor
    {
        Boolean Read<TClient>(TClient client) where TClient : IClient;
        void Process<TClient>(TClient client) where TClient : IClient;
        void Dispose();
    }
}
