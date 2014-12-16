using System;
using Throne.World.Network;

namespace Throne.World.Structures.Objects
{
    public class Creature : WorldObject
    {
        public Creature(UInt32 iD) : base(iD)
        {
        }
        public override void SpawnFor(WorldClient observer)
        {
            throw new NotImplementedException();
        }

        public override void DespawnFor(WorldClient observer)
        {
            throw new NotImplementedException();
        }
    }
}