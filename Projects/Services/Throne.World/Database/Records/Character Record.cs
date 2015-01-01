using System;
using System.Collections.Generic;
using Throne.Framework.Persistence.Mapping;
using Throne.World.Database.Records;
using Throne.World.Database.Records.Implementations;
using Throne.World.Structures.Objects;

namespace Throne.World.Records
{
    public class CharacterRecord : WorldDatabaseRecord
    {
        public virtual Int32 OwnerGuid { get; set; }
        public virtual UInt32 Guid { get; set; }
        public virtual String Name { get; set; }
        public virtual Int32 Look { get; set; }
        public virtual RoleAppearance Appearance { get; set; }
        public virtual Int16 Hairstyle { get; set; }
        public virtual Byte CurrentJob { get; set; }
        public virtual Byte PreviousJob { get; set; }
        public virtual Byte AncestorJob { get; set; }
        public virtual Byte Level { get; set; }
        public virtual UInt32 MapID { get; set; }
        public virtual UInt32 InstanceId { get; set; }
        public virtual Int16 X { get; set; }
        public virtual Int16 Y { get; set; }
        public virtual UInt32 EMoney { get; set; }
        public virtual UInt32 Money { get; set; }
        public virtual UInt64 Experience { get; set; }
        public virtual Int16 CrimeLevel { get; set; }
        public virtual UInt16 Strength { get; set; }
        public virtual UInt16 Agility { get; set; }
        public virtual UInt16 Spirit { get; set; }
        public virtual UInt16 Vitality { get; set; }
        public virtual UInt16 AttributePoints { get; set; }
        public virtual CharacterRecord Spouse { get; set; }
        public virtual DateTime? CreationTime { get; set; }
        public virtual String CreatorMacAddress { get; set; }
        public virtual IList<ItemRecord> ItemPayload { get; set; }
        public virtual IList<MailRecord> MailPayload { get; set; }

        public override void Update()
        {
            WorldServer.Instance.WorldDbContext.Update(this);
        }
    }

    public sealed class CharacterMapping : MappableObject<CharacterRecord>
    {
        public CharacterMapping()
        {
            Id(r => r.Guid);

            Map(r => r.OwnerGuid);
            Map(r => r.Name);
            Map(r => r.Look);
            Map(r => r.Hairstyle);
            Map(r => r.Appearance);
            Map(r => r.Level);
            Map(r => r.MapID);
            Map(r => r.InstanceId);
            Map(r => r.X);
            Map(r => r.Y);
            Map(r => r.Money);
            Map(r => r.EMoney);
            Map(r => r.CurrentJob);
            Map(r => r.PreviousJob);
            Map(r => r.AncestorJob);
            Map(r => r.Experience);
            Map(r => r.CrimeLevel);
            Map(r => r.Strength);
            Map(r => r.Agility);
            Map(r => r.Spirit);
            Map(r => r.Vitality);
            Map(r => r.AttributePoints);

            Map(r => r.CreationTime);
            Map(r => r.CreatorMacAddress);

            References(x => x.Spouse).Nullable();

            HasMany(r => r.ItemPayload).Not.LazyLoad().KeyColumn("OwnerId")
                .Inverse();

            HasMany(r => r.MailPayload).Not.LazyLoad().KeyColumn("recipientId")
                .Inverse();
        }
    }
}