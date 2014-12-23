using System;
using Throne.Framework.Persistence.Mapping;
using Throne.World.Database.Records.Implementations;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Storage;

namespace Throne.World.Records
{
    public class ItemRecord : WorldDatabaseRecord
    {
        public virtual UInt32 Guid { get; set; }
        public virtual DepsoitoryType Coffer { get; set; }
        public virtual Int32 Type { get; set; }
        public virtual Item.Positions Position { get; set; }
        public virtual Byte CraftLevel { get; set; }
        public virtual Int32 CraftProgress { get; set; }
        public virtual Byte FirstSlot { get; set; }
        public virtual Byte SecondSlot { get; set; }
        public virtual CharacterRecord Owner { get; set; }

        public override void Create()
        {
            WorldServer.Instance.WorldDbContext.Commit(this);
        }
    }

    public class ItemMapping : MappableObject<ItemRecord>
    {
        public ItemMapping()
        {
            Id(r => r.Guid).GeneratedBy.Assigned();

            References(r => r.Owner);

            Map(r => r.Type);
            Map(r => r.Position);
            Map(r => r.CraftLevel);
            Map(r => r.CraftProgress);
            Map(r => r.FirstSlot);
            Map(r => r.SecondSlot);
            Map(r => r.Coffer);
        }
    }
}