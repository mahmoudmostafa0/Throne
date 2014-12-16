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

        private readonly UInt32 id;

        public Orientation Direction;

        protected WorldObject(UInt32 ID)
        {
            id = ID;
        }

        public virtual String Name { get; set; }


        public Boolean IsPlayer
        {
            get { return id >= PlayerIdMin && id <= PlayerIdMax; }
        }

        public Boolean IsMonster
        {
            get { return id >= MonsterIdMin && id <= MonsterIdMax; }
        }

        public Boolean IsPet
        {
            get { return id >= PetIdMin && id <= PetIdMax; }
        }

        public Boolean IsCallPet
        {
            get { return id >= CallPetIdMin && id <= CallPetIdMax; }
        }

        public Boolean IsNPC
        {
            get { return id >= NpcIdMin && id <= NpcIdMax && !IsMonster && !IsPet; }
        }

        public Boolean IsTerrainNPC
        {
            get { return id >= DynaNpcIdMin && id <= DynaNpcIdMax; }
        }

        public virtual Location Location { get; set; }
        public abstract void SpawnFor(WorldClient observer);

        /// <summary>
        ///     Represents the unique identification number of an object in the region or game world.
        /// </summary>
        public UInt32 ID
        {
            get { return id; }
        }

        public abstract void DespawnFor(WorldClient observer);

        public static implicit operator Boolean(WorldObject obj)
        {
            return obj != null;
        }
    }
}