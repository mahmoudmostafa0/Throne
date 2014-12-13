using System;
using Throne.Shared.Network.Handling;
using Throne.Shared.Network.Transmission;

namespace Throne.World.Network.Handling
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class WorldPacketHandlerAttribute : PacketHandlerAttribute
    {
        public WorldPacketHandlerAttribute(PacketTypes typeId)
            : base(typeId)
        {
        }

        public override string ToString()
        {
            return "World packet handler";
        }
    }
}