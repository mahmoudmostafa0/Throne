using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class ItemAction
    {
        private void UseItem()
        {
            var item = Character.GetInventoryItem(Guid);
            if (!item) return;
            var pos = (Item.Positions) Argument;

            if (pos != Item.Positions.Inventory)
            Character.EquipGearSlot(item, pos);
        }
    }
}