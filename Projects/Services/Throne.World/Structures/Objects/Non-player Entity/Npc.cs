using System;
using Throne.World.Network;
using Throne.World.Network.Messages;

namespace Throne.World.Structures.Objects
{
    public partial class Npc : WorldObject
    {
        public Int16 Action;
        public UInt32 ID;
        public NpcInformation.Types Type;
        public UInt16 TypeAndFacing;
        public Int32 Unknown;
        public Int16 X, Y;

        public Npc(uint ID) : base(ID)
        {

        }

        public override void SpawnFor(WorldClient observer)
        {
            using (var pkt = new NpcInformation(this))
                observer.Send(pkt);
        }

        public override void DespawnFor(WorldClient observer)
        {
            throw new NotImplementedException(); // not sure how to implement it yet
        }
    }
}