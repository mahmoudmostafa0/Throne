using System;
using Throne.World.Network;

namespace Throne.World.Structures.Objects
{
    public sealed class DynamicNpc : Npc
    {
        public DynamicNpc(uint ID) : base(ID)
        {
        }

        public override void SpawnFor(WorldClient observer)
        {
            throw new NotImplementedException();
        }
    }
}
