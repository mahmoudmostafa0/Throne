using System;
using Throne.World.Structures.Objects;

namespace Throne.World.Scripting.Scripts
{
    /// <summary>
    /// Base class for external item scripts.
    /// </summary>
    public abstract class ItemScript : ScriptBase
    {
        /// <summary>
        /// The item ID hooked with this script.
        /// </summary>
        public Int32 HookedItemID;

        public virtual void OnUse(Character chr, Item item)
        { }
        public virtual void OnEquip(Character chr, Item item)
        { }
        public virtual void OnUnequip(Character chr, Item item)
        { }
        public virtual void OnCreation(Character chr, Item item)
        { }
        public virtual void OnPickup(Character chr, Item item)
        { }
        public virtual void OnDrop(Character chr, Item item)
        { }
        public virtual void OnEnterScreen(Character chr, Item item)
        { }
        public virtual void OnExitScreen(Character chr, Item item)
        { }

        public virtual void Hook(Int32 itemId)
        {
            HookedItemID = itemId;
        }
    }

    public sealed class DummyItemScript : ItemScript
    {
        
    }
}