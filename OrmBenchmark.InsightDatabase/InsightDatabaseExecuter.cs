using Insight.Database;
using Insight.Database.Providers.MySql;
using Insight.Database.Providers.MySqlConnector;
using Insight.Database.Providers.PostgreSQL;
using MySql.Data.MySqlClient;
using Npgsql;
using OrmBenchmark.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace OrmBenchmark.InsightDatabase
{
    public class InsightDatabaseExecuter : IOrmExecuter
    {
        private DbConnection conn;

        public string Name => "Insight Database";

        public DatabaseType DatabaseType { get; private set; }

        public void Dispose()
        {
            conn.Close();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.QuerySql<dynamic>("select * from posts");
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.QuerySql<Post>("select * from posts").ToList<IPost>();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return conn.QuerySql<dynamic>("select * from posts where Id=@Id", new { Id }).Single();
        }

        public IPost GetItemAsObject(int Id)
        {
            return conn.QuerySql<Post>("select * from posts where Id=@Id", new { Id }).Single();
        }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;

          conn =  DatabaseType.GetAndConfigureConnection(connectionString, (dbConnection, dbtype) =>
            {
                switch (dbtype)
                {
                    case DatabaseType.MySql:
                        MySqlInsightDbProvider.RegisterProvider();
                        break;
                    case DatabaseType.PostgreSql:
                        PostgreSQLInsightDbProvider.RegisterProvider();
                        break;
                    case DatabaseType.SqlServer:
                        SqlInsightDbProvider.RegisterProvider();
                        break;
                    case DatabaseType.MySqlConnector:
                        MySqlConnectorInsightDbProvider.RegisterProvider();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return dbConnection;
            });

          
        }


        public bool IsSupported(DatabaseType databaseType) => true;
    }
}
