using System;
using Throne.World.Network;
using Throne.World.Structures.Travel;

namespace Throne.World.Structures.Objects
{
    public abstract class WorldObject : IWorldObject
    {
        private const UInt32 NpcIdMin = 1;
        private const UInt32 DynaNpcIdMin = 100001;
        private const UInt32 DynaNpcIdMax = 199999;
        private const UInt32 MonsterIdMin = 400001;
        private const UInt32 MonsterIdMax = 499999;
        private const UInt32 PetIdMin = 500001;
        private const UInt32 PetIdMax = 599999;
        private const UInt32 NpcIdMax = 700000;
        private const UInt32 CallPetIdMin = 700001;
        private const UInt32 CallPetIdMax = 799999;
        public const UInt32 PlayerIdMin = 1000000, PlayerIdMax = 10000000;
        public const UInt32 ItemIdMin = 10000001, ItemIdMax = UInt32.MaxValue;

        private UInt32 _id;

        public Orientation Direction;

        protected WorldObject(UInt32 id)
        {
            _id = id;
        }

        public virtual String Name { get; set; }


        public Boolean IsPlayer
        {
            get { return _id >= PlayerIdMin && _id <= PlayerIdMax; }
        }

        public Boolean IsMonster
        {
            get { return _id >= MonsterIdMin && _id <= MonsterIdMax; }
        }

        public Boolean IsPet
        {
            get { return _id >= PetIdMin && _id <= PetIdMax; }
        }

        public Boolean IsCallPet
        {
            get { return _id >= CallPetIdMin && _id <= CallPetIdMax; }
        }

        public Boolean IsNPC
        {
            get { return _id >= NpcIdMin && _id <= NpcIdMax && !IsMonster && !IsPet; }
        }

        public Boolean IsTerrainNPC
        {
            get { return _id >= DynaNpcIdMin && _id <= DynaNpcIdMax; }
        }

        public virtual Location Location { get; set; }
        public abstract void SpawnFor(WorldClient observer);

        /// <summary>
        ///     Represents the unique identification number of an object in the region or game world.
        /// </summary>
        public UInt32 ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public abstract void DespawnFor(WorldClient observer);

        public static implicit operator Boolean(WorldObject obj)
        {
            return obj != null;
        }
    }
}