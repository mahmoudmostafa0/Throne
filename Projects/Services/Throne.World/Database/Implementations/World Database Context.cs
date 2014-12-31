using System.Collections.Generic;
using FluentNHibernate;
using Throne.Framework.Persistence;
using Throne.World.Records;

namespace Throne.World.Database.Records.Implementations
{
    public sealed class WorldDatabaseContext : GameDatabaseContext
    {
        public WorldDatabaseContext(DatabaseType dbType, string connStr)
            : base(dbType, connStr)
        {
        }
        protected override IEnumerable<IMappingProvider> CreateMappings()
        {
            yield return new AccountMapping();
            yield return new CharacterMapping();
            yield return new SerialGeneratorMapping();
            yield return new MapInfoMapping();
            yield return new ItemMapping();
            yield return new ServerInfoMapping();
            yield return new MailMapping();
        }
    }
}