using System;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class ItemAction
    {
        public ItemAction Equip(Item itm, Item.Positions pos)
        {
            ActionType = ItemActionType.Equip;
            Guid = itm.Guid;
            Argument = (UInt32)pos;
            return this;
        }
    }
}