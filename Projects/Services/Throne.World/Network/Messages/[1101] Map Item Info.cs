using System;
using Throne.Shared.Network.Transmission;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public sealed class MapItemInformation : WorldPacket
    {
        public enum MapItemTypes : short
        {
            None,
            DropItem,
            RemoveItem,
            ObtainItem,
            DetainItem,
            SkillEffect = 10,
            GroundEffect = 11,
            RemoveEffect = 12,
            ShowEffect = 13,
            VibrateScreen = 14,
        }

        public enum SkillTypes : short
        {
            TwilightDance = 40,
            DaggerStormV2,
            DaggerStormV3,
            DaggerStorm = 46
        }

        public MapItemInformation(Item item, Boolean remove = false) : base(PacketTypes.MapItem, 48)
        {
            WriteInt(Environment.TickCount);
            WriteUInt(item.Guid);
            WriteInt(item.Type); // item type
            WriteShort(item.Location.Position.X);
            WriteShort(item.Location.Position.Y);
            WriteShort(6); // color
            WriteInt((Int16)(remove ? MapItemTypes.RemoveItem : MapItemTypes.DropItem)); // action
            WriteUInt(0);
            WriteInt(0); // time created
            WriteInt(0); // currency value
            WriteShort(0); // item type
        }
}
}
