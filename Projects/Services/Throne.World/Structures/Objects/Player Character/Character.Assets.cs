using System;
using System.Linq;
using Throne.Shared;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Storage;

namespace Throne.World.Structures.Objects
{
    partial class Character
    {
        /// TODO: If no space to add a new item in inventory, send to mailbox. (quest rewards and such)

        #region Currency
        public Int32 Money
        {
            get { return Record.Money; }
            set
            {
                Record.Money = value;
                Record.Update();
            }
        }

        public Int32 EMoney
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

        public void AddInventoryItem(Item itm)
        {
            if (_inventory.Add(itm))
                User.Send(itm);
            //else send to mailbox
        }

        public Item RemoveInventoryItem(UInt32 guid)
        {
            using (var pkt = new ItemAction().Remove(guid))
                User.Send(pkt);

            return _inventory.Remove(guid);
        }

        public Item GetInventoryItem(UInt32 guid)
        {
            var item = _inventory.Items.SingleOrDefault(i => i.Guid == guid);
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

        public GearSlot getGearSlot(Item.Positions pos)
        {
            if (pos == Item.Positions.Inventory)
                throw new ArgumentException("{0} is not part of the gear collection.".Interpolate(pos));
            return _gear[pos];
        }

        public Int32 getGearType(Item.Positions pos)
        {
            return getGearSlot(pos).ContainedType;
        }

        public UInt32 getGearGuid(Item.Positions pos)
        {
            return getGearSlot(pos).ContainedGuid;
        }

        public void EquipGearSlot(Item item, Item.Positions pos)
        {
            var slot = getGearSlot(pos);
            if (!slot.Equip(item)) return;

            using (var pkt = new ItemAction().Equip(item, pos))
                User.Send(pkt);

            item.Position = pos;

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
            using (var pkt = new ItemAction().Unequip((uint)slot.Slot, slot.ContainedGuid))
                User.Send(pkt);

            var item = slot.Unequip();
            if (item)
            {
                item.Position = Item.Positions.Inventory;
                AddInventoryItem(item);
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

        #region Depositories

        

        #endregion

    }
}