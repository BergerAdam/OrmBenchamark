using Dapper;
using OrmBenchmark.Core;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OrmBenchmark.Dapper
{
    public class DapperExecuter : IOrmExecuter
    {
        private IDbConnection conn;
        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "Dapper Query (Non Buffered)";
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
            object param = new { Id = Id };
            return conn.Query<Post>("select * from Posts where Id=@Id", param, buffered: false).Single();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            object param = new { Id = Id };
            return conn.Query("select * from Posts where Id=@Id", param, buffered: false).Single();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.Query<Post>("select * from Posts", null, buffered: false).AsList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.Query("select * from Posts", null, buffered: false).AsList();
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