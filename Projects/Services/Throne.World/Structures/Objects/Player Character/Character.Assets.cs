using System;
using System.Linq;
using Throne.Framework;
using Throne.Framework.Network.Transmission.Stream;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Storage;

namespace Throne.World.Structures.Objects
{
    partial class Character
    {
        public Stream ItemStream
        {
            get { return new Stream().Join((Byte[][])_inventory).Join((Byte[][])_gear); }
        }

        /// TODO: If no space to add a new item in inventory, send to mailbox. (quest rewards and such)
        public void AddItem(Item itm)
        {
            itm.OwnerInfo = Record;
            MoveToInventory(itm);
            User.Send(itm);
        }

        public Item RemoveItem(Item itm)
        {
            itm.OwnerInfo = null;
            if (itm.Position > Item.Positions.Inventory)
                UnequipGearSlot(GetGearSlot(itm.Position));
            return MoveFromInventory(itm.ID);
        }

        #region Currency

        public UInt32 Money
        {
            get { return Record.Money; }
            set
            {
                Record.Money = value;
                Record.Update();
            }
        }

        public uint EMoney
        {
            get { return Record.EMoney; }
            set
            {
                Record.EMoney = value;
                Record.Update();
            }
        }

        #endregion Currency

        #region Inventory

        private readonly Inventory _inventory;

        public void MoveToInventory(Item itm)
        {
            itm.Position = Item.Positions.Inventory;
            _inventory[itm.ID] = itm;
        }

        public Item MoveFromInventory(UInt32 guid)
        {
            using (ItemAction pkt = new ItemAction().Remove(guid))
                User.Send(pkt);

            return _inventory.Remove(guid);
        }

        public Item GetInventoryItem(UInt32 guid)
        {
            Item item = _inventory[guid];
            if (!item)
                User.Send(StrRes.CMSG_InventoryNoItem);
            return item;
        }

        public Boolean AdequateInventorySpace(Int32 forCount)
        {
            if (_inventory.AdequateSpace(forCount))
                return true;

            User.Send(StrRes.CMSG_NotEnoughInventorySpace.Interpolate(forCount));
            return false;
        }

        #endregion Inventory

        #region Gear

        private readonly Gear _gear;

        public GearSlot GetGearSlot(Item.Positions pos)
        {
            if (pos == Item.Positions.Inventory)
                throw new ArgumentException("{0} is not part of the gear collection.".Interpolate(pos));
            return _gear[pos];
        }

        public Int32 GetGearType(Item.Positions pos)
        {
            return GetGearSlot(pos).ContainedType;
        }

        public UInt32 GetGearGuid(Item.Positions pos)
        {
            return GetGearSlot(pos).ContainedGuid;
        }

        public void EquipGearSlot(Item item, Item.Positions pos)
        {
            GearSlot slot = GetGearSlot(pos);
            if (!slot.Equip(item)) return;

            item.Position = pos;
            User.Send(new ItemAction().Equip(item));
            User.Send(new ItemAction().SendGear(this));


            switch (pos)
            {
                case Item.Positions.Headgear:
                case Item.Positions.Armor:
                case Item.Positions.RightHand:
                case Item.Positions.LeftHand:
                case Item.Positions.Garment:
                case Item.Positions.Mount:
                case Item.Positions.MountArmor:
                case Item.Positions.RightWeaponAccessory:
                case Item.Positions.LeftWeaponAccessory:
                    SendToLocal();
                    break;
            }
        }

        public void UnequipGearSlot(GearSlot slot)
        {
            Item item = slot.Unequip();
            if (item)
            {
                MoveToInventory(item);
                User.Send(new ItemAction().Unequip(slot.Slot, item.ID));
                User.Send(new ItemAction().SendGear(this));
            }

            switch (slot.Slot)
            {
                case Item.Positions.Headgear:
                case Item.Positions.Armor:
                case Item.Positions.RightHand:
                case Item.Positions.LeftHand:
                case Item.Positions.Garment:
                case Item.Positions.Mount:
                case Item.Positions.MountArmor:
                case Item.Positions.RightWeaponAccessory:
                case Item.Positions.LeftWeaponAccessory:
                    SendToLocal();
                    break;
            }
        }

        #endregion Gear
    }
}