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
    public class Letter
    {
        protected readonly MailRecord Record;

        public Letter(MailRecord record)
        {
            Record = record;
        }

        public UInt32 ID
        {
            get { return Record.Guid; }
        }

        public String Sender
        {
            get { return Record.Sender; }
        }

        public String Header
        {
            get { return Record.Header; }
        }

        public String Content
        {
            get { return Record.Content; }
        }

        public Boolean Opened
        {
            get { return Record.Opened; }
            set
            {
                Record.Opened = value;
                Record.Update();
            }
        }

        public DateTime Creation
        {
            get { return Record.Creation; }
        }

        public CharacterRecord Recipient
        {
            get { return Record.Recipient; }
        }

        public virtual Boolean ParcelPost
        {
            get { return false; }
        }

        public virtual UInt32 Money
        {
            get { return 0; }
            set { throw new InvalidOperationException(); }
        }

        public virtual UInt32 EMoney
        {
            get { throw new InvalidOperationException(); }
            set { throw new InvalidOperationException(); }
        }

        public virtual Item Item
        {
            get { throw new InvalidOperationException(); }
            set { throw new InvalidOperationException(); }
        }
    }
}