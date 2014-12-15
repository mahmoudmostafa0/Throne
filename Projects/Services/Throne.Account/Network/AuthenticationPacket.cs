using System;
using Throne.Shared.Network.Connectivity;

namespace Throne.Shared.Network.Transmission
{
    public class AuthenticationPacket : Packet
    {
        protected AuthenticationPacket()
        {
        }

        public AuthenticationPacket(IConvertible type, short len)
            : base(type, len)
        {
        }

        public AuthenticationPacket(byte[] payload)
            : base(payload)
        {
        }
    }
}