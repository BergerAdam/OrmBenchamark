using OrmBenchmark.Core;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OrmBenchmark.OrmLite
{
    public class OrmLiteNoQueryExecuter : IOrmExecuter
    {
        private IDbConnection conn;
        private OrmLiteConnectionFactory dbFactory;
        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "Orm Lite (No Query)";
            }
        }

        public void Init(string connectionStrong, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            dbFactory = new OrmLiteConnectionFactory(connectionStrong, SelectDialect());
            conn = dbFactory.Open();
        }

        private IOrmLiteDialectProvider SelectDialect()
        {
            switch (DatabaseType)
            {
                case DatabaseType.MySql:
                    return MySqlDialect.Provider;

                case DatabaseType.MySqlConnector:
                    return MySqlConnectorDialect.Provider;

                case DatabaseType.PostgreSql:
                    {
                        var provider = PostgreSqlDialect.Provider;
                        provider.NamingStrategy = new AliasNamingStrategy();
                        return provider;
                    }
                case DatabaseType.SqlServer:
                    return SqlServerDialect.Provider;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IPost GetItemAsObject(int Id)
        {
            return conn.Single<Post>(new { Id });
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            var q = conn.From<Post>()
                .Where(p => p.Id == Id)
                .Select(p => new
                {
                    p.Id,
                    p.Text,
                    p.CreationDate,
                    p.LastChangeDate,
                    p.Counter1,
                    p.Counter2,
                    p.Counter3,
                    p.Counter4,
                    p.Counter5,
                    p.Counter6,
                    p.Counter7,
                    p.Counter8,
                    p.Counter9
                });

            return conn.Single<dynamic>(q);
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.Select<Post>().AsList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            var q = conn.From<Post>()
                .Select(p => new
                {
                    p.Id,
                    p.Text,
                    p.CreationDate,
                    p.LastChangeDate,
                    p.Counter1,
                    p.Counter2,
                    p.Counter3,
                    p.Counter4,
                    p.Counter5,
                    p.Counter6,
                    p.Counter7,
                    p.Counter8,
                    p.Counter9
                });

            return conn.Select<dynamic>(q).AsList();
        }

        public void Dispose()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Dispose();
        }

        public bool IsSupported(DatabaseType databaseType) => true;
    }
}