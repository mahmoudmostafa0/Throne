using System;
using Throne.Framework.Network.Connectivity;

namespace Throne.Framework.Network.Transmission
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