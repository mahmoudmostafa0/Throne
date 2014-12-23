using System;
using Throne.World.Network;
using Throne.World.Network.Messages;
using Throne.World.Structures.Travel;

namespace Throne.World.Structures.Objects.Actors
{
    public partial class Npc : WorldObject
    {
        public Npc(UInt32 id)
            : base(id)
        {
            Look = new Model(1, Orientation.Northeast);
        }

        public Npc() : base(0)
        {
            Look = new Model(1, Orientation.Northeast);
        }

        public override void SpawnFor(WorldClient observer)
        {
            using (WorldPacket pkt = this)
                observer.Send(pkt);
        }

        public override void DespawnFor(WorldClient observer)
        {
            throw new NotImplementedException(); // not sure how to implement it yet
        }

        public static implicit operator WorldPacket(Npc npc)
        {
            return new NpcInformation(npc);
        }

        public void SpawnAtLocation()
        {
            Location.Map.AddNpc(this);
        }
    }
}