using Throne.World.Structures.World;

namespace Throne.World.Network.Messages
{
    public partial class ItemAction
    {
        private void Drop()
        {
            var item = Character.RemoveInventoryItem(Guid);
            if (!item) return;

            var location = Character.Location.RandomLocalCopy(CellType.Item, 5);

            item.OwnerInfo = null;
            item.Location = location;
            location.Map.AddItem(item);

#if DEBUG
            Character.User.Send("Item dropped.");
#endif
        }
    }
}