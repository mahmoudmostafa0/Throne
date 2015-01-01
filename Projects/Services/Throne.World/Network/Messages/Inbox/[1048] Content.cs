using System;
using System.Text;
using Throne.Framework.Network.Transmission;

namespace Throne.World.Network.Messages.Inbox
{
    public sealed class Content : WorldPacket
    {
        public Content(String text, UInt32 id) 
            : base(PacketTypes.MailContent, text.Length + 8 + 8)
        {
            WriteUInt(id);
            WriteString(text);
            WriteInt(0);
        }
    }
}