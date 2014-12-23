using System.Collections.Generic;
using FluentNHibernate;

namespace Throne.Login.Records.Implementations
{
    using Framework.Persistence;

    public sealed class AccountDatabaseContext : GameDatabaseContext
    {
        public AccountDatabaseContext(DatabaseType dbType, string connStr)
            : base(dbType, connStr)
        {
        }

        protected override IEnumerable<IMappingProvider> CreateMappings()
        {
            yield return new AccountMapping();
        }
    }
}