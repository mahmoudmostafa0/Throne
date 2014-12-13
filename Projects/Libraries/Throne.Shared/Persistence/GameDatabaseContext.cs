using System.Collections.Generic;
using System.Diagnostics.Contracts;
using FluentNHibernate.Conventions;
using Throne.Shared.Persistence.Conventions.Identity;
using Throne.Shared.Persistence.Conventions.Naming;
using Throne.Shared.Persistence.Conventions.Properties;
using Throne.Shared.Persistence.Conventions.Relationships;

namespace Throne.Shared.Persistence
{
    public abstract class GameDatabaseContext : DatabaseContext
    {
        protected GameDatabaseContext(DatabaseType type, string connString)
            : base(type, connString)
        {
            Contract.Requires(!string.IsNullOrEmpty(connString));
        }

        protected override sealed IEnumerable<IConvention> CreateConventions()
        {
            yield return new IdConvention();
            yield return new IdGenerationConvention();

            yield return new ClassNameConvention();
            yield return new ForeignKeyNameConvention();

            yield return new PropertyNullableConvention();
            yield return new PropertyUpdateConvention();

            yield return new ReferenceConvention();
            yield return new RelationshipCascadeConvention();
            yield return new RelationshipFetchConvention();
            yield return new RelationshipLazyLoadConvention();
        }
    }
}