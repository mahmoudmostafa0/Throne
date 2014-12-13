using System;
using Throne.World.Network;

namespace Throne.World.Structures.Objects
{
    public class Role : WorldObject
    {
        protected Model _look;

        public Role(uint ID)
            : base(ID)
        {
        }

        public override void SpawnTo(WorldClient observer)
        {

        }

        public virtual Model Look
        {
            get { return _look; }
            set { _look = value; }
        }

        public static implicit operator Boolean(Role role)
        {
            return role != null;
        }
    }
}
