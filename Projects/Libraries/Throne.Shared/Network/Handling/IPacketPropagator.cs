
using Throne.Shared.Network.Connectivity;

namespace Throne.Shared.Network.Handling
{
    public interface IPacketPropagator
    {
        void Handle(IClient client, short typeId, byte[] payload, short length);
    }
}