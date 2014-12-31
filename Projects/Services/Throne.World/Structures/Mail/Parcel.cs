using System;
using Throne.World.Database.Records;
using Throne.World.Structures.Objects;

namespace Throne.World.Structures.Mail
{
    /// <summary>
    ///     A package containing assets to be delivered to a player via mail.
    ///     Parcels are saved immediately after modification because they always contain assets.
    /// </summary>
    internal class Parcel : Letter
    {
        public Parcel(MailRecord record) : base(record)
        {
        }

        public UInt32 Money
        {
            get { return Record.Money; }
            set { Record.Money = value; Record.UpdateNow(); }
        }

        public UInt32 EMoney
        {
            get { return Record.EMoney; }
            set { Record.EMoney = value; Record.UpdateNow(); }
        }

        public Item Item
        {
            get { return new Item(Record.Item); }
            set
            {
                Record.Item = value.Record;
                Record.UpdateNow();
            }
        }

        public override Boolean ParcelPost
        {
            get { return true; }
        }
    }
}