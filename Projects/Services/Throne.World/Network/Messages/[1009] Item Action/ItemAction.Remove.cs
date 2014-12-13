using System;

namespace Throne.World.Network.Messages
{
    public partial class ItemAction
    {
        public ItemAction Remove(UInt32 guid)
        {
            ActionType = ItemActionType.Remove;
            Guid = guid;
            return this;
        }
    }
}
