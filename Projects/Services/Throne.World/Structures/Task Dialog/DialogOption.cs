
using System;
using Throne.World.Network;
using Throne.World.Network.Messages;
using Throne.World.Structures.Objects.Actors;

namespace Throne.World.Structures
{
    public sealed class DialogOption : Dialog
    {
        public Byte Op { get; set; }
        public String Name { get; set; }

        public DialogOption(String name, Byte op)
        {
            Name = name;
            Op = op;
        }

        protected override WorldPacket MakePacket()
        {
            return new TaskDialog(Name.Length * 2 + 2).Option(Op, Name, Name);
        }
    }
}
