﻿using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Context;
using NHibernate.Driver;
using NHibernate.Driver.MySqlConnector;
using NHibernate.Transform;
using OrmBenchmark.Core;
using System;
using System.Collections.Generic;

namespace OrmBenchmark.NHibernate
{
    public class NHibernateSqlQueryExecutor : IOrmWithCacheExecuter
    {
        public string Name => "NHibernate (SqlQuery)";
        public DatabaseProvider DatabaseProvider { get; private set; }

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

        public void Init(string connectionString, DatabaseProvider databaseType)
        {
            DatabaseProvider = databaseType;

            SessionFactory = Fluently.Configure()
              .Database(CreateConfiguration(connectionString))
              .CurrentSessionContext<ThreadStaticSessionContext>()
              .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<Post>())
              .BuildSessionFactory();
        }

        private IPersistenceConfigurer CreateConfiguration(string connectionString)
        {
            switch (DatabaseProvider)
            {
                case DatabaseProvider.Npgsql:
                    return PostgreSQLConfiguration.Standard.ConnectionString(connectionString);

                case DatabaseProvider.MySqlData:
                    return MySQLConfiguration.Standard.Driver<MySqlDataDriver>().ConnectionString(connectionString);

                case DatabaseProvider.SystemData:
                    return MsSqlConfiguration.MsSql2012.Driver<SqlClientDriver>().ConnectionString(connectionString);

                case DatabaseProvider.MicrosoftData:
                    return MsSqlConfiguration.MsSql2012.Driver<MicrosoftDataSqlClientDriver>().ConnectionString(connectionString);

                case DatabaseProvider.MySqlConnector:
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
            return Session.CreateSQLQuery("select * from Posts").List<dynamic>();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return Session.CreateSQLQuery("select * from Posts")
                .SetResultTransformer(Transformers.AliasToBean<Post>())
                .List<Post>();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return Session.CreateSQLQuery("select * from Posts where Id = :Id").SetParameter("Id", Id).UniqueResult<dynamic>();
        }

        public IPost GetItemAsObject(int Id)
        {
            return Session.CreateSQLQuery("select * from Posts where Id = :Id").SetParameter("Id", Id)
                .SetResultTransformer(Transformers.AliasToBean<Post>())
                .UniqueResult<Post>();
        }

        public void ClearCache()
        {
            SessionFactory.Evict(typeof(Post));
        }

        public bool IsSupported(DatabaseProvider databaseType) => true;
    }
}