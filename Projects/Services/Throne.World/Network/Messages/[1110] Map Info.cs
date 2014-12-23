using System;
using Throne.Framework.Network.Transmission;
using Throne.World.Structures.World;

namespace Throne.World.Network.Messages
{
    public sealed class MapInfo : WorldPacket
    {
        public MapInfo(Map map) : base(PacketTypes.MapInfo, 28)
        {
            WriteUInt(map.Id);
            WriteUInt(map.Document);
            WriteLong((Int64)map.Attributes); //permission mask
        }
    }
}