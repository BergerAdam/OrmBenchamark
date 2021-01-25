using Insight.Database;
using Insight.Database.Providers.MySql;
using Insight.Database.Providers.MySqlConnector;
using Insight.Database.Providers.PostgreSQL;
using OrmBenchmark.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace OrmBenchmark.InsightDatabase
{
    public class InsightDatabaseExecuter : IOrmExecuter
    {
        private DbConnection conn;

        public string Name => "Insight Database";

        public DatabaseProvider DatabaseProvider { get; private set; }

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

        public void Init(string connectionString, DatabaseProvider databaseType)
        {
            DatabaseProvider = databaseType;

            conn = DatabaseProvider.GetAndConfigureConnection(connectionString, (dbConnection, dbtype) =>
             {
                 switch (dbtype)
                 {
                     case DatabaseProvider.MySqlData:
                         MySqlInsightDbProvider.RegisterProvider();
                         break;

                     case DatabaseProvider.Npgsql:
                         PostgreSQLInsightDbProvider.RegisterProvider();
                         break;

                     case DatabaseProvider.SystemData:
                     case DatabaseProvider.MicrosoftData:
                         SqlInsightDbProvider.RegisterProvider();
                         break;

                     case DatabaseProvider.MySqlConnector:
                         MySqlConnectorInsightDbProvider.RegisterProvider();
                         break;

                     default:
                         throw new ArgumentOutOfRangeException();
                 }

                 return dbConnection;
             });
        }

        public bool IsSupported(DatabaseProvider databaseType) => true;
    }
}