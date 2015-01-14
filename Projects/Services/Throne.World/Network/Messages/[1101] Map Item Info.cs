using System;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.Framework.Security.Permissions;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    [Handling.WorldPacketHandler(PacketTypes.MapItem, Permission = typeof(AuthenticatedPermission))]
    public sealed class MapItemInformation : WorldPacket
    {
        public MapItemInformation(byte[] payload)
            : base(payload)
        {
        }
        public int Timestamp;
        public uint UID, ItemID;
        public ushort X, Y;
        public Item.Color Color;
        public MapItemTypes Type;
        private Character Character;
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
        public override bool Read(IClient client)
        {
            Timestamp = ReadInt();
            UID = ReadUInt();
            ItemID = ReadUInt();
            X = ReadUShort();
            Y = ReadUShort();
            Color = (Item.Color)ReadUShort();
            Type = (MapItemTypes)ReadUShort();
            return true;
        }
        public override void Handle(IClient client)
        {
            Character = ((WorldClient)client).Character;
            if (Character.Location.Position.X == X && Character.Location.Position.Y == Y) //it should check if char is trading too.
            {
                if (!Character.AdequateInventorySpace(1))
                    return;
                var map = Character.Location.Map;
                var item = map.GetItem(UID);
                if (item == null) return;
                item.OwnerInfo = Character.Record;
                map.RemoveItem(item);
                Character.MoveToInventory(item);
            }
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
