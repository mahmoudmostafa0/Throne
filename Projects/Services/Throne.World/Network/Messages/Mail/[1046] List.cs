using System;
using System.Collections.Generic;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.World.Network.Handling;
using Throne.World.Structures.Mail;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages.Mail
{
    [WorldPacketHandler(PacketTypes.MailList)]
    public class List : WorldPacket
    {
        public List(Byte[] array)
            : base(array)
        {
        }

        public List(Int32 count, Int32 page, Int32 pages)
            : base(PacketTypes.MailList, 88*count + 16 + 8)
        {           
            WriteInt(count);
            WriteInt(10);
            WriteByte(27);
            WriteInt(0);

        }

        public List MakeList(IEnumerable<Letter> list, int page)
        {
 
            foreach (var mail in list)
            {
                WriteUInt(mail.ID);
                WriteString(mail.Sender);
                SeekForward(32 - mail.Sender.Length);
                WriteString(mail.Header);
                SeekForward(32 - mail.Header.Length);
                WriteUInt(mail.ParcelPost ? mail.Money : (uint) (mail.Opened ? 0 : 1));
                WriteUInt(mail.ParcelPost ? mail.EMoney : 0);
                WriteInt((int) (DateTime.Now - mail.Creation).TotalSeconds);
                WriteUInt(mail.ParcelPost ? mail.Item.ID : 0);
                WriteInt(0);
            }
            return this;
        }

        public override bool Read(IClient client)
        {
            Character c = ((WorldClient) client).Character;
            var test = new List(3, 3, 1);
            //test.Test =
            //    new Letter(new MailRecord()
            //    {
            //        Content = "asssaads",
            //        Creation = DateTime.Now.AddYears(-21),
            //        Header = "sdfafs",
            //        Guid = 123,
            //        Opened = true,
            //        Recipient = c.Record,
            //        Sender = "me"
            //    });
            c.User.Send(test);
            return false;
        }
    }
}