using System;
using Throne.Framework.Network.Handling;
using Throne.Framework.Network.Transmission;

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