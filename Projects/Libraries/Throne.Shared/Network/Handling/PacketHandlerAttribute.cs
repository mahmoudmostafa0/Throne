using System;

namespace Throne.Shared.Network.Handling
{
    public abstract class PacketHandlerAttribute : Attribute
    {
        protected PacketHandlerAttribute(Enum type)
        {
            PacketTypeId = type;
        }

        public Enum PacketTypeId { get; private set; }

        public Type Permission { get; set; }
    }
}