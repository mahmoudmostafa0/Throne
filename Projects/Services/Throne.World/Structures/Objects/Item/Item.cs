using System;
using System.Collections.Generic;
using Throne.Shared.Network.Transmission.Stream;
using Throne.World.Network;
using Throne.World.Network.Messages;
using Throne.World.Records;

namespace Throne.World.Structures.Objects
{
    public partial class Item : WorldObject
    {
        public enum Positions : byte
        {
            Inventory = 0,

            //has alts
            Headgear = 1,
            Lavalier = 2,
            Armor = 3,
            RightHand = 4,
            LeftHand = 5,
            Band = 6,
            Talisman = 7,
            Boots = 8,
            Garment = 9,

            //no alts
            Fan = 10,
            Tower = 11,
            Mount = 12,
            RightWeaponAccessory = 15,
            LeftWeaponAccessory = 16,
            MountArmor = 17,
            MountTalisman = 18,

            AlternateHeadgear = 21,
            AlternateLavalier = 22,
            AlternateArmor = 23,
            AlternateRightHand = 24,
            AlternateLeftHand = 25,
            AlternateBand = 26,
            AlternateTalisman = 27,
            AlternateBoots = 28,
            AlternateGarment = 29
        }

        private readonly ItemRecord Record;

        public Item(ItemRecord record)
            : base(record.Guid)
        {
            Record = record;
        }


        public UInt32 Guid
        {
            get { return Record.Guid; }
        }

        public CharacterRecord OwnerInfo
        {
            get { return Record.Owner; }
            set
            {
                Record.Owner = value;
                Record.Update();
            }
        }

        public Int32 Type
        {
            get { return Record.Type; }
        }

        public Positions Position
        {
            get { return Record.Position; }
            set
            {
                Record.Position = value;
                Record.Update();
            }
        }

        public static Stream ToStream(IEnumerable<Item> toSend)
        {
            var _stream = new Stream();
            foreach (Item item in toSend)
                _stream.Join(item);
            return _stream;
        }

        public static implicit operator Boolean(Item item)
        {
            return item != null;
        }

        public static implicit operator Byte[](Item toAdd)
        {
            return new ItemInformation(toAdd);
        }

        public override void SpawnFor(WorldClient observer)
        {
            using (var pkt = new MapItemInformation(this))
                observer.Send(pkt);
        }

        public override void DespawnFor(WorldClient observer)
        {
            using (var pkt = new MapItemInformation(this, true))
                observer.Send(pkt);
        }
    }
}