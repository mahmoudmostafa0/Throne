namespace Throne.World.Structures.Objects
{
    public abstract class Role : WorldObject
    {
        protected Model _look;

        public Role(uint ID)
            : base(ID)
        {
        }

        public virtual Model Look
        {
            get { return _look; }
            set { _look = value; }
        }
    }
}
