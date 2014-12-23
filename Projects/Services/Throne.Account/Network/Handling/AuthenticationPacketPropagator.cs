using Throne.Framework.Network.Handling;
using Throne.Framework.Network.Transmission;

namespace Throne.Login.Network.Handling
{
    public sealed class AuthenticationPacketPropagator : PacketPropagatorBase<
        AuthenticationPacketHandlerAttribute,
        AuthenticationPacket>
    {
    }
}