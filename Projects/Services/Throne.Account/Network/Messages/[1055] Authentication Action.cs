using System;
using Throne.Shared.Network.Transmission;

namespace Throne.Login.Network.Messages
{
    public sealed class AuthenticationAction : AuthenticationPacket
    {
        public enum Type
        {
            Banned = 0,
            InvalidCredentials = 1,
            Forward = 2,
            AccountBanned = 25,
            AccountPermanentlyBanned = 26,
            OutdatedClient = 61,
            AuthenticationSessionInvalid = 73
        }

        public AuthenticationAction(Int32 authResponse, Int32 sessionId = 0, UInt16 port = 0, String host = "")
            : base(PacketTypes.AuthenticationAction, 36)
        {
            WriteInt(sessionId);
            WriteInt(authResponse);
            WriteInt(port);
            SeekForward(4).WriteString(host);
        }
    }
}