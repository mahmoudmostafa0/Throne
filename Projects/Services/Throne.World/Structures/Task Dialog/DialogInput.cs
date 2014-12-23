using System;
using Throne.World.Network;
using Throne.World.Network.Messages;

namespace Throne.World.Structures.Task_Dialog
{
    public sealed class DialogInput : Dialog
    {
        public DialogInput(String name, Byte op, UInt16 size)
        {
            Name = name;
            Op = op;
            Size = size;
        }

        public String Name { get; set; }
        public UInt16 Size { get; set; }
        public Byte Op { get; set; }

        protected override WorldPacket MakePacket()
        {
            return new TaskDialog(Name.Length).Input(Op, Size, Name);
        }
    }
}