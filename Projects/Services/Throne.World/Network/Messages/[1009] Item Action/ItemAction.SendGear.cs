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
                WriteUInt(chr.GetGearGuid(Item.Positions.AlternateHeadgear));
                WriteUInt(chr.GetGearGuid(Item.Positions.AlternateLavalier));
                WriteUInt(chr.GetGearGuid(Item.Positions.AlternateArmor));
                WriteUInt(chr.GetGearGuid(Item.Positions.AlternateRightHand));
                WriteUInt(chr.GetGearGuid(Item.Positions.AlternateLeftHand));
                WriteUInt(chr.GetGearGuid(Item.Positions.AlternateBand));
                WriteUInt(chr.GetGearGuid(Item.Positions.Talisman));
                WriteUInt(chr.GetGearGuid(Item.Positions.AlternateBoots));
                WriteUInt(chr.GetGearGuid(Item.Positions.AlternateGarment));
            }
            else
            {
                WriteUInt(chr.GetGearGuid(Item.Positions.Headgear));
                WriteUInt(chr.GetGearGuid(Item.Positions.Lavalier));
                WriteUInt(chr.GetGearGuid(Item.Positions.Armor));
                WriteUInt(chr.GetGearGuid(Item.Positions.RightHand));
                WriteUInt(chr.GetGearGuid(Item.Positions.LeftHand));
                WriteUInt(chr.GetGearGuid(Item.Positions.Band));
                WriteUInt(chr.GetGearGuid(Item.Positions.Talisman));
                WriteUInt(chr.GetGearGuid(Item.Positions.Boots));
                WriteUInt(chr.GetGearGuid(Item.Positions.Garment));
            }

            WriteUInt(chr.GetGearGuid(Item.Positions.RightWeaponAccessory));
            WriteUInt(chr.GetGearGuid(Item.Positions.LeftWeaponAccessory));
            WriteUInt(chr.GetGearGuid(Item.Positions.MountArmor));
            WriteUInt(chr.GetGearGuid(Item.Positions.MountTalisman));
            return this;
        }
    }
}