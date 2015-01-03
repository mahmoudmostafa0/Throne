using System;
using Throne.World.Network;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;

namespace Throne.World.Properties
{
    public static class Constants
    {
        public static class CharacterManagementMessages
        {
            public static readonly WorldPacket NameInvalid = new ChatMessage(MessageChannel.Popup, StrRes.CMSG_CharacterNameUnavailable);
            public static readonly WorldPacket AnswerOk = new ChatMessage(MessageChannel.Popup, "ANSWER_OK");
        }

        public static class LoginMessages
        {
            public static readonly WorldPacket AnswerOk = new ChatMessage(MessageChannel.Login, "ANSWER_OK");
            public static readonly WorldPacket BadAuthentication = new ChatMessage(MessageChannel.Login, StrRes.CMSG_AuthenticationFailed);
            public static readonly WorldPacket NewRole = new ChatMessage(MessageChannel.Login, "NEW_ROLE");
            public static readonly WorldPacket ServerInfo = new ServerInfo();
        }
    }
}