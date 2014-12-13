using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;

/// <summary>
/// Throne - ScentSword skillbook script.
/// Executed on item useage, teaches the skill, deletes the skillbook item if the skill was learnable.
/// </summary>
public sealed class ScentSword : ItemScript
{
    public override void Load()
    {
        //Hooked item IDs must be unique.
        //An error will be thrown if another loaded
        //script hooks the same ID unless the Remove or
        //Override attribute is used on the class.
        
        Hook(itemId: 725010);
    }

    public override void OnUse(Character chr, Item item)
    {
        /*
         * skill learnable?
         * skill learned?
         * delete item.
         * else send message that the skill is not learnable, what are the requirements?
         */
    }
}
