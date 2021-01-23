using Dapper;
using Dapper.Contrib.Extensions;
using OrmBenchmark.Core;
using System.Collections.Generic;
using System.Data;

namespace OrmBenchmark.Dapper
{
    public class DapperContribExecuter : IOrmExecuter
    {
        private IDbConnection conn;
        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "Dapper Contrib";
            }
        }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            conn = databaseType.GetAndConfigureConnection<IDbConnection>(connectionString, (dbConnection) =>
            {
                dbConnection.Open();
                return dbConnection;
            });
        }

        public IPost GetItemAsObject(int Id)
        {
            return conn.Get<Post>(Id);
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return null;
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.GetAll<Post>().AsList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return null;
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