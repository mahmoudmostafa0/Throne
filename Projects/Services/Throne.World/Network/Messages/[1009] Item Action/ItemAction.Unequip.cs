using System;
using Throne.Framework;
using Throne.World.Properties.Settings;
using Throne.World.Security;
using Throne.World.Structures;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class ItemAction
    {
        private void UnequipRequest()
        {
            //TODO: Set suspicious where necessary.

            var position = (Item.Positions) Argument;
            GearSlot slot = Character.getGearSlot(position);

            if (slot.Empty || !slot.Item) 
                throw new MildViolation(StrRes.VMSG_NoEquip.Interpolate(Character.Name));

            Item item = slot.Item;

            if (item.Position != position)
                throw new SevereViolation(StrRes.VMSG_ItemSlotMismatch.Interpolate(Character.Name));

            if (!position.IsValid())
                throw new SevereViolation("Incorrect item position to unequip.");

            if (item.Guid != Guid)
                throw new SevereViolation("The unequip request guid did not match the equipped item guid.");

            if (!Character.AdequateInventorySpace(1))
                return;

            Character.UnequipGearSlot(slot);
        }

        public ItemAction Unequip(UInt32 position, UInt32 guid)
        {
            ActionType = ItemActionType.Unequip;
            Argument = position;
            Guid = guid;
            return this;
        }
    }
}