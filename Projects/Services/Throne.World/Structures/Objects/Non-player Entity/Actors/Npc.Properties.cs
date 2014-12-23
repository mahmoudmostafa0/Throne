using System;
using Throne.World.Network.Messages;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Travel;

namespace Throne.World.Structures.Objects.Actors
{
    partial class Npc
    {
        public Int16 Action;
        public UInt32 Health;
        public Model Look;
        public UInt32 MaxHealth;
        public String Name = "the nameless one";
        public NpcScript Script;
        public NpcInformation.Types Type;


        public struct Model
        {
            public Int16 Face;
            public UInt16 Id;

            public Model(UInt16 mesh, Orientation facing)
            {
                Id = (UInt16) (mesh*10 + facing);
                Face = -1;
            }

            public UInt16 Mesh
            {
                get { return (UInt16) ((Id/10)%1000); }
                set { Id = (UInt16) (value*10 + Facing); }
            }

            public Orientation Facing
            {
                get { return (Orientation) (Id%10); }
                set { Id = (UInt16) (Mesh*10 + value); }
            }

            public static implicit operator UInt16(Model mdl)
            {
                return mdl.Id;
            }

            public static implicit operator Int16(Model mdl)
            {
                return mdl.Face;
            }
        }
    }
}