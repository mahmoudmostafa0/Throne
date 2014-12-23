using System;
using Throne.Framework.Network.Transmission;

namespace Throne.Login.Network.Messages
{
    public sealed class AuthCipherSeed : AuthenticationPacket
    {
        public AuthCipherSeed(Int32 seed)
            : base(PacketTypes.LoginSeed, 8)
        {
            WriteInt(seed);
        }
    }
}