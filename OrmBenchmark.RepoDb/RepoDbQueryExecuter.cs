using OrmBenchmark.Core;
using RepoDb;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace OrmBenchmark.RepoDb
{
    public class RepoDbQueryExecuter : IOrmExecuter
    {
        private DbConnection conn;

        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "RepoDb Query";
            }
        }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            conn = DatabaseType.GetAndConfigureConnection(connectionString, (connection, dbType) =>
            {
                switch (dbType)
                {
                    case DatabaseType.MySql:
                        MySqlBootstrap.Initialize();
                        break;

                    case DatabaseType.PostgreSql:
                        PostgreSqlBootstrap.Initialize();
                        break;

                    case DatabaseType.SqlServer:
                        SqlServerBootstrap.Initialize();
                        break;

                    case DatabaseType.MySqlConnector:
                        MySqlConnectorBootstrap.Initialize();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                connection.Open();
                return connection;
            });
        }

        public void Dispose()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            CommandTextCache.Flush();

            conn.Dispose();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.QueryAll<Post>().AsList();
        }

        public IPost GetItemAsObject(int Id)
        {
            return conn.Query<Post>(Id).Single();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.QueryAll("Posts").AsList(); ;
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return conn.Query("Posts", Id).Single();
        }

        public bool IsSupported(DatabaseType databaseType) => true;
    }
}