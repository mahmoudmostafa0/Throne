using System;
using Throne.World.Database.Records;
using Throne.World.Records;
using Throne.World.Structures.Objects;

namespace Throne.World.Structures.Mail
{
    /// <summary>
    ///     A message which is delivered to a player via mail.
    ///     Lazily saved, delayed delivery
    /// </summary>
    public class Mail
    {
        private readonly MailRecord _record;

        public Mail(MailRecord record)
        {
            _record = record;
        }

        public UInt32 ID
        {
            get { return _record.Guid; }
        }

        public String Sender
        {
            get { return _record.Sender; }
        }

        public String Header
        {
            get { return _record.Header; }
        }

        public String Content
        {
            get { return _record.Content; }
        }

        public Boolean Opened
        {
            get { return _record.Opened; }
            set
            {
                _record.Opened = value;
                _record.Update();
            }
        }

        public DateTime Creation
        {
            get { return _record.Creation; }
        }

        public CharacterRecord Recipient
        {
            get { return _record.Recipient; }
        }

        public Boolean ContainsAttachment
        {
            get { return Money + EMoney > 0 || Item; }
        }

        public UInt32 Money
        {
            get { return _record.Money; }
            set
            {
                _record.Money = value;
                _record.UpdateNow();
            }
        }

        public UInt32 EMoney
        {
            get { return _record.EMoney; }
            set
            {
                _record.EMoney = value;
                _record.UpdateNow();
            }
        }

        public Item Item
        {
            get { return _record.Item != null ? new Item(_record.Item) : null; }
            set
            {
                _record.Item = value ? value.Record : null;
                _record.UpdateNow();
            }
        }

        public void Delete()
        {
            _record.Delete();
        }
    }
}