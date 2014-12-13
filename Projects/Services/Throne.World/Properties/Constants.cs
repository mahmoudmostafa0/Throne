using System;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;

namespace Throne.World.Properties
{
    public static class Constants
    {
        public static class CharacterManagementMessages
        {
            public static readonly Byte[] NameInvalid = new ChatMessage(MessageChannel.Popup, StrRes.CMSG_CharacterNameUnavailable);
            public static readonly Byte[] AnswerOk = new ChatMessage(MessageChannel.Popup, "ANSWER_OK");
        }

        public static class LoginMessages
        {
            public static readonly Byte[] AnswerOk = new ChatMessage(MessageChannel.Login, "ANSWER_OK");
            public static readonly Byte[] BadAuthentication = new ChatMessage(MessageChannel.Login, StrRes.CMSG_AuthenticationFailed);
            public static readonly Byte[] NewRole = new ChatMessage(MessageChannel.Login, "NEW_ROLE");
            public static readonly Byte[] ServerInfo = new ServerInfo();
        }
    }
}