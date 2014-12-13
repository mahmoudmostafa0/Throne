using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using NHibernate;
using NHibernate.Linq;
using Throne.Shared.Persistence.Schema;
using Throne.Shared.Threading.Actors;

namespace Throne.Shared.Persistence
{
    public abstract class DatabaseContext : Actor<DatabaseContext>
    {
        protected DatabaseContext(DatabaseType type, string connString)
        {
            Configure(type, connString);
        }

        protected internal NHibernate.Cfg.Configuration Configuration { get; private set; }

        protected ISessionFactory SessionFactory { get; private set; }

        public SchemaInfo Schema { get; private set; }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(SessionFactory != null);
            Contract.Invariant(Schema != null);
            Contract.Invariant(Configuration != null);
        }

        protected override void Dispose(bool disposing)
        {
            SessionFactory.Dispose();

            base.Dispose(disposing);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Switch statements are not that evil..."
            )]
        private static IPersistenceConfigurer CreateConfiguration(DatabaseType type, string connString)
        {
            Contract.Requires(!string.IsNullOrEmpty(connString));
            Contract.Ensures(Contract.Result<IPersistenceConfigurer>() != null);

            IPersistenceConfigurer config;

            switch (type)
            {
                case DatabaseType.DB2:
                    config = DB2Configuration.Standard.ConnectionString(connString);
                    break;
                case DatabaseType.MsSql2005:
                    config = MsSqlConfiguration.MsSql2005.ConnectionString(connString);
                    break;
                case DatabaseType.MsSql2008:
                    config = MsSqlConfiguration.MsSql2008.ConnectionString(connString);
                    break;
                case DatabaseType.MsSqlCe:
                    config = MsSqlCeConfiguration.Standard.ConnectionString(connString);
                    break;
                case DatabaseType.MySql:
                    config = MySQLConfiguration.Standard.ConnectionString(connString);
                    break;
                case DatabaseType.Oracle10:
                    config = OracleClientConfiguration.Oracle10.ConnectionString(connString);
                    break;
                case DatabaseType.OracleData10:
                    config = OracleDataClientConfiguration.Oracle10.ConnectionString(connString);
                    break;
                case DatabaseType.PostgreSql:
                    config = PostgreSQLConfiguration.Standard.ConnectionString(connString);
                    break;
                case DatabaseType.SQLite:
                    config = SQLiteConfiguration.Standard.ConnectionString(connString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            Contract.Assume(config != null);
            return config;
        }

        /// <summary>
        ///     Configures the DatabaseContext.
        /// </summary>
        /// <param name="dbType">The type of SQL server to connect to.</param>
        /// <param name="connString">The connection string to be used to establish a connection.</param>
        private void Configure(DatabaseType dbType, string connString)
        {
            FluentConfiguration fluent = Fluently.Configure();
            fluent.Database(CreateConfiguration(dbType, connString));

            foreach (Type mappingType in CreateMappings().Select(mapping => mapping.GetType()))
            {
                Type type = mappingType;
                fluent.Mappings(x => x.FluentMappings.Add(type));
            }

            foreach (IConvention convention in CreateConventions())
            {
                IConvention conv = convention;
                fluent.Mappings(x => x.FluentMappings.Conventions.Add(conv));
            }

            NHibernate.Cfg.Configuration config = fluent.BuildConfiguration();
            Contract.Assume(config != null);
            Configuration = config;

            ISessionFactory factory = fluent.BuildSessionFactory();
            Contract.Assume(factory != null);
            SessionFactory = factory;

            Schema = new SchemaInfo(this);
        }

        protected abstract IEnumerable<IMappingProvider> CreateMappings();

        protected virtual IEnumerable<IConvention> CreateConventions()
        {
            Contract.Ensures(Contract.Result<IEnumerable<IConvention>>() != null);

            yield break;
        }

        /// <summary>
        ///     Creates a disposable database session.
        /// </summary>
        /// <returns>A unit-of-work session which should be disposed ASAP.</returns>
        protected ISession CreateSession()
        {
            Contract.Ensures(Contract.Result<ISession>() != null);

            ISession session = SessionFactory.OpenSession();
            Contract.Assume(session != null);
            return session;
        }

        /// <summary>
        ///     Adds an entity and its persistent children to the database.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        public void Commit(object item)
        {
            using (var session = CreateSession())
            {
                using (session.BeginTransaction())
                {
                    session.Persist(item);
                    session.Transaction.Commit();
                }
            }
        }

        /// <summary>
        ///     Adds a list of entities and their persistent children to the database.
        /// </summary>
        /// <param name="itemsToSave">The entities to add.</param>
        public void Commit(IEnumerable<object> itemsToSave)
        {
            Contract.Requires(itemsToSave != null);

            using (ISession session = CreateSession())
            {
                using (session.BeginTransaction())
                {
                    foreach (object item in itemsToSave)
                        session.Persist(item);

                    session.Transaction.Commit();
                }
            }
        }

        /// <summary>
        ///     Saves the updated values of an entity and its persistent children to the database.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        public void Update(object item)
        {
            using (ISession session = CreateSession())
            {
                using (session.BeginTransaction())
                {
                    session.Update(item);
                    session.Transaction.Commit();
                }
            }
        }

        /// <summary>
        ///     Saves the updated values of a list of entities and their persistent children to the database.
        /// </summary>
        /// <param name="itemsToSave">The entities to update.</param>
        public void Update(IEnumerable<object> itemsToSave)
        {
            Contract.Requires(itemsToSave != null);

            using (ISession session = CreateSession())
            {
                using (session.BeginTransaction())
                {
                    foreach (object item in itemsToSave)
                        session.Update(item);

                    session.Transaction.Commit();
                }
            }
        }

        /// <summary>
        ///     Deletes an entity from the database.
        /// </summary>
        /// <param name="item">The entity to delete.</param>
        public void Delete(object item)
        {
            Contract.Requires(item != null);

            using (ISession session = CreateSession())
            {
                using (session.BeginTransaction())
                {
                    session.Delete(item);
                    session.Transaction.Commit();
                }
            }
        }

        /// <summary>
        ///     Deletes a list of entities from the database.
        /// </summary>
        /// <param name="itemsToDelete">The entities to delete.</param>
        public void Delete(IEnumerable<object> itemsToDelete)
        {
            Contract.Requires(itemsToDelete != null);

            using (ISession session = CreateSession())
            {
                using (session.BeginTransaction())
                {
                    foreach (object item in itemsToDelete)
                        session.Delete(item);

                    session.Transaction.Commit();
                }
            }
        }

        /// <summary>
        ///     Retrieves a list of entities matching the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be retrieved.</typeparam>
        /// <param name="criteria">The criteria to use when searching.</param>
        /// <returns>A list of all entities meeting the specified criteria.</returns>
        public IEnumerable<T> Find<T>(Func<T, bool> criteria)
            where T : class
        {
            using (ISession session = CreateSession())
                return session.Query<T>().Where(criteria).ToList();
        }

        /// <summary>
        ///     Retrieves all entities of a given type.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be retrieved.</typeparam>
        /// <returns>A list of all entities of the given type.</returns>
        public IEnumerable<T> FindAll<T>()
            where T : class
        {
            return Find<T>(x => true);
        }
    }

    [Serializable]
    public enum DatabaseType : byte
    {
        DB2,
        MsSql2005,
        MsSql2008,
        MsSqlCe,
        MySql,
        Oracle10,
        OracleData10,
        PostgreSql,
        SQLite
    }
}