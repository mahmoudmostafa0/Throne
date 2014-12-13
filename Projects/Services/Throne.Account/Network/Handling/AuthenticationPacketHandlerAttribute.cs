using System;
using Throne.Shared.Network.Handling;
using Throne.Shared.Network.Transmission;

namespace Throne.Login.Network.Handling
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AuthenticationPacketHandlerAttribute : PacketHandlerAttribute
    {
        public AuthenticationPacketHandlerAttribute(PacketTypes typeId)
            : base(typeId)
        {
        }

        public override string ToString()
        {
            return "Auth packet handler";
        }
    }
}