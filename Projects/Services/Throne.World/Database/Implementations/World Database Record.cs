using System;
using Throne.Framework.Persistence.Interfaces;

namespace Throne.World.Database.Records.Implementations
{
    public abstract class WorldDatabaseRecord : IActiveRecord
    {
        public virtual void Create()
        {
            WorldServer.Instance.WorldDbContext.PostAsync(x => x.Commit(this));
        }

        public virtual void Update()
        {
            WorldServer.Instance.WorldDbContext.PostAsync(x => x.Update(this));
        }

        public virtual void Delete()
        {
            WorldServer.Instance.WorldDbContext.PostAsync(x => x.Delete(this));
        }

        public static implicit operator Boolean(WorldDatabaseRecord wdbRecord)
        {
            return wdbRecord != null;
        }
    }
}