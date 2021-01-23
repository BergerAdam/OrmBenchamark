using OrmBenchmark.Core;
using RepoDb;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OrmBenchmark.RepoDb
{
    public class RepoDbExecuter : IOrmExecuter
    {
        private IDbConnection conn;

        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "RepoDb";
            }
        }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            conn = DatabaseType.GetAndConfigureConnection<IDbConnection>(connectionString, (connection, dbType) =>
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

        public IPost GetItemAsObject(int Id)
        {
            object param = new { Id = Id };
            return conn.ExecuteQuery<Post>("select * from Posts where Id=@Id", param).Single();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            object param = new { Id = Id };
            return conn.ExecuteQuery("select * from Posts where Id=@Id", param).Single();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.ExecuteQuery<Post>("select * from Posts").AsList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.ExecuteQuery("select * from Posts").AsList();
        }

        public bool IsSupported(DatabaseType databaseType) => true;
    }
}