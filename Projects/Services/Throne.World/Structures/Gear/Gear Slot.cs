using System;
using Throne.World.Structures.Objects;

namespace Throne.World.Structures
{
    public class GearSlot
    {
        private readonly Item.Positions _pos;

        public GearSlot(Item.Positions pos, Item item = null)
        {
            _pos = pos;
            Item = item;
        }

        public Item Item { get; private set; }

        public Int32 ContainedType
        {
            get { return Item ? Item.Type : 0; }
        }

        public UInt32 ContainedGuid
        {
            get { return Item ? Item.Guid : 0; }
        }

        public Boolean Empty
        {
            get { return !Item; }
        }

        public Item.Positions Slot
        {
            get { return _pos; }
        }

        public Boolean Equip(Item item)
        {
            if (Item)
                return false;
            Item = item;
            return true;
        }

        public Item Unequip()
        {
            var item = Item;
            Item = null;
            return item;
        }
    }
}