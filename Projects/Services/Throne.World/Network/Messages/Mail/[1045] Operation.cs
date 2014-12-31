using System;
using Throne.Framework.Network.Transmission;
using Throne.World.Network.Handling;

namespace Throne.World.Network.Messages.Mail
{
    [WorldPacketHandler(PacketTypes.MailOperation)]
    public sealed class Operation : WorldPacket
    {
        public enum Types
        {
            None,
            Open,
            Delete,
            RemoveMoney,
            RemoveEMoney,
            RemoveItem,
            RemoveAttachment
        }

        public Operation(Byte[] array) : base(array)
        {
        }

        public Operation(Types type, UInt32 mailId)
            : base(PacketTypes.MailOperation, 20)
        {
            WriteInt((int) type);
            WriteUInt(mailId);
        }
    }
}