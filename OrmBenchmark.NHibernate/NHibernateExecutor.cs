using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Context;
using NHibernate.Driver;
using NHibernate.Driver.MySqlConnector;
using OrmBenchmark.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrmBenchmark.NHibernate
{
    public class NHibernateExecutor : IOrmWithCacheExecuter
    {
        public string Name => "NHibernate";
        public DatabaseType DatabaseType { get; private set; }

        private ISession Session
        {
            get
            {
                if (!CurrentSessionContext.HasBind(SessionFactory))

                    CurrentSessionContext.Bind(SessionFactory.OpenSession());

                return SessionFactory.GetCurrentSession();
            }
        }

        private ISessionFactory SessionFactory;

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;

            SessionFactory = Fluently.Configure()
              .Database(CreateConfiguration(connectionString))
              .CurrentSessionContext<ThreadStaticSessionContext>()
              .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<Post>())
              .BuildSessionFactory();
        }

        private IPersistenceConfigurer CreateConfiguration(string connectionString)
        {
            switch (DatabaseType)
            {
                case DatabaseType.PostgreSql:
                    return PostgreSQLConfiguration.Standard.ConnectionString(connectionString);

                case DatabaseType.MySql:
                    return MySQLConfiguration.Standard.Driver<MySqlDataDriver>().ConnectionString(connectionString);

                case DatabaseType.SqlServer:
                    return MsSqlConfiguration.MsSql2012.ConnectionString(connectionString);

                case DatabaseType.MySqlConnector:
                    return MySQLConfiguration.Standard.Driver<MySqlConnectorDriver>().ConnectionString(connectionString);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Dispose()
        {
            SessionFactory.Close();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return null;
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return Session.Query<Post>().ToList();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return null;
        }

        public IPost GetItemAsObject(int Id)
        {
            return Session.Get<Post>(Id);
        }

        public void ClearCache()
        {
            SessionFactory.Evict(typeof(Post));
        }

        public bool IsSupported(DatabaseType databaseType) => true;
    }
}