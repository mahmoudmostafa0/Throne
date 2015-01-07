using System;
using Throne.World.Network;
using Throne.World.Network.Messages;

namespace Throne.World.Structures
{
    public sealed class DialogMessage : Dialog
    {
        public DialogMessage(String msg)
        {
            Message = msg;
        }

        public String Message { get; set; }

        protected override WorldPacket MakePacket()
        {
            return new TaskDialog(Message.Length).Message(Message);
        }
    }
}