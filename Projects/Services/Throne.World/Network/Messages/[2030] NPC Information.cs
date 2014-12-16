using System;
using Throne.Shared.Network.Transmission;

namespace Throne.World.Network.Messages
{
    public sealed class NpcInformation : WorldPacket
    {
        /// <summary>
        ///     Thanks to whoever for correctly(?) naming each of these types, I'm not sure who you are. -scott
        /// </summary>
        public enum Types : short
        {
            Stun = 0,
            Shop = 1,
            Talker = 2,
            Storage = 3,
            Trunk = 4,
            Beautician = 5,
            Upgrader = 6,
            Socketer = 7,
            Bless = 8,
            Statuary = 9,
            RoleStatue = 10,
            RolePlayer = 11,
            RoleHero = 12,
            RoleMoney = 13,
            RoleBooth = 14,
            RoleTransport = 15,
            RoleBoothFlag = 16,
            RoleMouse = 17,
            RoleMagicItem = 18,
            RoleDice = 19,
            RoleShelf = 20,
            RoleWeaponTarget = 21,
            RoleMagicTarget = 22,
            RoleBowTarget = 23,
            RoleTarget = 24,
            RoleTerrain = 25,
            RoleCityGate = 26,
            RoleNeighborDoor = 27,
            RoleCallPet = 28,
            RoleClan = 31
        }

        public Int16 Action;
        public UInt32 ID;
        public Int32 Unknown;
        public UInt16 LookFace;
        public Types Type;
        public Int16 X, Y;

        public NpcInformation()
            : base(PacketTypes.NpcInformation, 34)
        {
        }

        protected override byte[] Build()
        {
            WriteUInt(ID); // local id?
            WriteUInt(ID); // task id?
            WriteInt(Unknown); // unknown
            WriteShort(X);
            WriteShort(Y);
            WriteUShort(LookFace);
            WriteShort((Int16) Type);
            WriteShort(Action); // action
            return base.Build();
        }
    }
}