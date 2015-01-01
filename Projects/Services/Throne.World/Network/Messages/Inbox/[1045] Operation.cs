using System;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.World.Managers;
using Throne.World.Network.Handling;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages.Inbox
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

        public UInt32 MailId;
        public Types Type;

        public Operation(Byte[] array) : base(array)
        {
        }

        public Operation(Types type, UInt32 mailId)
            : base(PacketTypes.MailOperation, 20)
        {
            Type = type;
            MailId = mailId;
        }

        public override bool Read(IClient client)
        {
            Type = (Types) ReadInt();
            MailId = ReadUInt();
            return true;
        }

        public override void Handle(IClient client)
        {
            var chr = ((WorldClient) client).Character;
            switch (Type)
            {
                case Types.Open:
                    MailManager.Instance.Open(chr, MailId);
                    break;
                case Types.Delete:
                    if (MailManager.Instance.Delete(chr, MailId))
                        client.Send(this);
                    break;
                case Types.RemoveItem:
                    if (MailManager.Instance.RemoveItemAttachment(chr, MailId))
                        client.Send(this);
                    break;
                case Types.RemoveMoney:
                    if (MailManager.Instance.RemoveMoneyAttachment(chr, MailId))
                        client.Send(this);
                    break;
                case Types.RemoveEMoney:
                    if (MailManager.Instance.RemoveEMoneyAttachment(chr, MailId))
                        client.Send(this);
                    break;
            }
        }

        protected override byte[] Build()
        {
            Seek(4);
            WriteInt((Int32) Type);
            WriteUInt(MailId);
            return base.Build();
        }
    }
}