using System;
using Throne.Shared.Exceptions;
using Throne.Shared.Persistence.Mapping;
using Throne.World.Database.Records.Implementations;
using Throne.World.Structures.World;

namespace Throne.World.Database.Records
{
    public class MapInfoRecord : WorldDatabaseRecord
    {

        public virtual UInt32 MapId { get; set; }
        public virtual UInt32 DocumentId { get; set; }
        public virtual MapAttribute Attributes { get; set; }
        public virtual UInt32 SpawnMapId { get; set; }
        public virtual Int16 SpawnPointX { get; set; }
        public virtual Int16 SpawnPointY { get; set; }
        public virtual String EnvironmentColor { get; set; }
        public virtual String DataMapPath { get; set; }

        public override void Create()
        {
            try
            {
                WorldServer.Instance.WorldDbContext.Commit(this);
            }
            catch (Exception e)
            {
                ExceptionManager.RegisterException(e);
            }
        }
    }

    public sealed class MapInfoMapping : MappableObject<MapInfoRecord>
    {
        public MapInfoMapping()
        {
            Id(r => r.MapId).GeneratedBy.Assigned();

            Map(r => r.DocumentId);
            Map(r => r.Attributes);
            Map(r => r.SpawnMapId);
            Map(r => r.SpawnPointX);
            Map(r => r.SpawnPointY);
            Map(r => r.EnvironmentColor).Default("'White'");
            Map(r => r.DataMapPath);
        }
    }
}
