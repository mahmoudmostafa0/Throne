using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class ItemAction
    {
        public ItemAction SendGear(Character chr)
        {
            Guid = chr.ID;
            Argument = (uint) (chr.AlternateGearActive ? 1 : 0);
            ActionType = ItemActionType.Gear;

            Seek(36);
            if (chr.AlternateGearActive)
            {
                WriteUInt(chr.getGearGuid(Item.Positions.AlternateHeadgear));
                WriteUInt(chr.getGearGuid(Item.Positions.AlternateLavalier));
                WriteUInt(chr.getGearGuid(Item.Positions.AlternateArmor));
                WriteUInt(chr.getGearGuid(Item.Positions.AlternateRightHand));
                WriteUInt(chr.getGearGuid(Item.Positions.AlternateLeftHand));
                WriteUInt(chr.getGearGuid(Item.Positions.AlternateBand));
                WriteUInt(chr.getGearGuid(Item.Positions.Talisman));
                WriteUInt(chr.getGearGuid(Item.Positions.AlternateBoots));
                WriteUInt(chr.getGearGuid(Item.Positions.AlternateGarment));
            }
            else
            {
                WriteUInt(chr.getGearGuid(Item.Positions.Headgear));
                WriteUInt(chr.getGearGuid(Item.Positions.Lavalier));
                WriteUInt(chr.getGearGuid(Item.Positions.Armor));
                WriteUInt(chr.getGearGuid(Item.Positions.RightHand));
                WriteUInt(chr.getGearGuid(Item.Positions.LeftHand));
                WriteUInt(chr.getGearGuid(Item.Positions.Band));
                WriteUInt(chr.getGearGuid(Item.Positions.Talisman));
                WriteUInt(chr.getGearGuid(Item.Positions.Boots));
                WriteUInt(chr.getGearGuid(Item.Positions.Garment));
            }

            WriteUInt(chr.getGearGuid(Item.Positions.RightWeaponAccessory));
            WriteUInt(chr.getGearGuid(Item.Positions.LeftWeaponAccessory));
            WriteUInt(chr.getGearGuid(Item.Positions.MountArmor));
            WriteUInt(chr.getGearGuid(Item.Positions.MountTalisman));
            return this;
        }
    }
}