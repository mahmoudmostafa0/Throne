using System;

namespace Throne.World.Structures.Objects
{
    /// <summary> This part of the Item class defines the composition of a usable game item. </summary>
    partial class Item
    {
        public Byte CraftLevel
        {
            get { return Record.CraftLevel; }
            set
            {
                Record.CraftLevel = value;
                Record.Update();
            }
        }
        public Int32 CraftProgress
        {
            get { return Record.CraftProgress; }
            set
            {
                Record.CraftProgress = value;
                Record.Update();
            }
        }
        public Byte FirstSlot
        {
            get { return Record.FirstSlot; }
            set
            {
                Record.SecondSlot = value;
                Record.Update();
            }
        }
        public Byte SecondSlot
        {
            get { return Record.SecondSlot; }
            set
            {
                Record.SecondSlot = value;
                Record.Update();
            }
        }
    }
}
