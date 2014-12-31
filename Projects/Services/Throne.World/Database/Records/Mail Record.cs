using System;
using Throne.Framework.Persistence.Mapping;
using Throne.World.Database.Records.Implementations;
using Throne.World.Records;

namespace Throne.World.Database.Records
{
    public class MailRecord : WorldDatabaseRecord
    {
        public virtual UInt32 Guid { get; set; }
        public virtual String Sender { get; set; }
        public virtual String Header { get; set; }
        public virtual String Content { get; set; }
        public virtual UInt32 EMoney { get; set; }
        public virtual UInt32 Money { get; set; }
        public virtual DateTime Creation { get; set; }
        public virtual Boolean Opened { get; set; }
        public virtual ItemRecord Item { get; set; }
        public virtual CharacterRecord Recipient { get; set; }

        public virtual void UpdateNow()
        {
            WorldServer.Instance.WorldDbContext.Update(this);
        }
    }

    public class MailMapping : MappableObject<MailRecord>
    {
        public MailMapping()
        {
            Id(r => r.Guid).GeneratedBy.Assigned();

            Map(r => r.Sender);
            Map(r => r.Header);
            Map(r => r.Content);
            Map(r => r.EMoney);
            Map(r => r.Money);
            Map(r => r.Opened);
            Map(r => r.Creation);

            References(r => r.Item);
            References(r => r.Recipient);
        }
    }
}