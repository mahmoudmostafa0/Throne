using Throne.Framework.Network.Transmission;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public sealed class ItemInformation : WorldPacket
    {
        public ItemInformation(Item item, Item.Mode mode = Item.Mode.AddOrMove)
            : base(PacketTypes.ItemInformation, 72 + 8)
        {
            lock (item)
            {
                WriteUInt(item.Guid);
                WriteInt(item.Type);
                WriteShort(100); // durability
                WriteShort(100); // max durability
                WriteShort((short)mode);
                WriteShort((short) item.Position); // position
                WriteInt(0); // socket progress, offset also used for steed rgb
                WriteByte(item.FirstSlot);
                WriteByte(item.SecondSlot);
                SeekForward(2 * sizeof(byte)); // idk
                WriteInt(0); // item effect (poison, hp, mana, shield)
                SeekForward(sizeof(byte)); // idk
                WriteByte(item.CraftLevel);
                WriteByte(0); // bless, next red value for steed
                WriteBoolean(false); // bound
                WriteByte(0); // hp add, next blue value for steed, may only be a single byte                
                WriteShort(0); //unknown
                WriteByte(0); // unknown
                WriteInt(0); // anti monster, next green value for steed
                WriteShort(0); // suspicious flags
                WriteShort(0); // lock flags
                WriteInt(6); // item color
                WriteInt(item.CraftProgress);
                WriteInt(0); // inscription
                WriteInt(0); // time left in seconds
                WriteInt(0); // expiry
                WriteShort(1); // stack size, default is 1
                WriteShort(0); // idk
            }
        }
    }
}
