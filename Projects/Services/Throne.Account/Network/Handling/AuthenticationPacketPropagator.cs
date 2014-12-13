using Throne.Shared.Network.Handling;
using Throne.Shared.Network.Transmission;

namespace Throne.Login.Network.Handling
{
    public sealed class AuthenticationPacketPropagator : PacketPropagatorBase<
        AuthenticationPacketHandlerAttribute,
        AuthenticationPacket>
    {
    }
}