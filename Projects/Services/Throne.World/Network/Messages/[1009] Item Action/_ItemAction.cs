using System;
using System.Collections.Generic;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Security.Permissions;
using Throne.Framework.Network.Transmission;
using Throne.World.Network.Handling;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    [WorldPacketHandler(PacketTypes.ItemAction, Permission = typeof(AuthenticatedPermission))]
    public sealed partial class ItemAction : WorldPacket
    {
        public ItemActionType ActionType;
        public UInt32 Argument;
        public List<UInt32> ArgumentEx;
        public Int32 ArgumentExCount;
        private Character Character;

        public UInt32 Guid;
        public Int32 TimeStamp;
        public Int32 dwParam1;
        public Int32 dwParam2;

        /// <summary>
        ///     Incoming constructor.
        /// </summary>
        /// <param name="array">Incoming byte array.</param>
        public ItemAction(byte[] array)
            : base(array)
        {
        }

        /// <summary>
        ///     Outgoing Action message.
        /// </summary>
        public ItemAction()
            : base(PacketTypes.ItemAction, 100)
        {
        }

        public override bool Read(IClient client)
        {
            SeekForward(sizeof(Int32));
            Guid = ReadUInt();
            Argument = ReadUInt();
            ActionType = (ItemActionType)ReadInt();
            TimeStamp = ReadInt();
            ArgumentExCount = ReadInt();
            dwParam1 = ReadInt();
            dwParam2 = ReadInt();

            SeekForward(14 * sizeof(Int32)); //equipment list
            SeekForward(sizeof(Int32)); //pad?

            var exCount = ArgumentExCount;
            for (ArgumentEx = new List<UInt32>(ArgumentExCount); exCount > 0; exCount--)
                ArgumentEx.Add(ReadUInt());

            return true;
        }

       
        public override void Handle(IClient client)
        {
            Character = ((WorldClient)client).Character;

            switch (ActionType)
            {
                case ItemActionType.Ping:
                    PingResponse();
                    break;
                case ItemActionType.DropItem:
                    Drop();
                    break;
                case ItemActionType.Use:
                    UseItem();
                    break;
                case ItemActionType.Unequip:
                    UnequipRequest();
                    break;
            }
        }

        protected override byte[] Build()
        {
            Seek(4);
            WriteInt(Environment.TickCount);
            WriteUInt(Guid);
            WriteUInt(Argument);
            WriteInt((int)ActionType);
            WriteInt(Environment.TickCount);
            return base.Build();
        }
    }
}