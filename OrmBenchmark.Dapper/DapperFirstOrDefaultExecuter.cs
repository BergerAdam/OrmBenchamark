using Dapper;
using MySql.Data.MySqlClient;
using Npgsql;
using OrmBenchmark.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OrmBenchmark.Dapper
{
    public class DapperFirstOrDefaultExecuter : IOrmExecuter
    {
        IDbConnection conn;
        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "Dapper Query (First Or Default)";
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
            return conn.QueryFirstOrDefault<Post>("select * from Posts where Id=@Id", new { Id});
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return conn.QueryFirstOrDefault("select * from Posts where Id=@Id", new { Id });
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return null;
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
