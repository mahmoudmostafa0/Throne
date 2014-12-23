
using Throne.Framework.Network.Connectivity;

namespace Throne.Framework.Network.Handling
{
    public interface IPacketPropagator
    {
        void Handle(IClient client, short typeId, byte[] payload, short length);
    }
}