﻿using Dapper;
using MySql.Data.MySqlClient;
using Npgsql;
using OrmBenchmark.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OrmBenchmark.Dapper
{
    public class DapperBufferedExecuter : IOrmExecuter
    {
        IDbConnection conn;

        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "Dapper Query (Buffered)";
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
            return conn.Query<Post>("select * from Posts where Id=@Id", new { Id }, buffered: true).Single();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return conn.Query("select * from Posts where Id=@Id", new { Id }, buffered: true).Single();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.Query<Post>("select * from Posts", null, buffered: true).AsList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.Query("select * from Posts", null, buffered: true).AsList();
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
