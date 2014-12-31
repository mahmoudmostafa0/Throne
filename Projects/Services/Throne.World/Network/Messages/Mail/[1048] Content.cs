using System;
using Throne.Framework.Network.Transmission;

namespace Throne.World.Network.Messages.Mail
{
    public sealed class Content : WorldPacket
    {
        public Content(String text, UInt32 id) 
            : base(PacketTypes.MailContent, text.Length + 4 + 8)
        {
            WriteUInt(id);
            WriteString(text);
        }
    }
}