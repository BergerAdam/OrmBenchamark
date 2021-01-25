using OrmBenchmark.Core;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrmBenchmark.SqlSugar
{
    public class SqlSugarExecuter : IOrmExecuter
    {
        public string Name => "SqlSugar";
        private SqlSugarClient db;
        public DatabaseProvider DatabaseProvider { get; private set; }

        public void Init(string connectionString, DatabaseProvider databaseType)
        {
            DatabaseProvider = databaseType;
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connectionString,
                DbType = GetDbType(),
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
        }

        private DbType GetDbType()
        {
            switch (DatabaseProvider)
            {
                case DatabaseProvider.Npgsql:
                    return DbType.PostgreSQL;

                case DatabaseProvider.MySqlData:
                    return DbType.MySql;

                case DatabaseProvider.SystemData:
                    return DbType.SqlServer;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return db.SqlQueryable<dynamic>("select * from Posts").ToList();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return db.SqlQueryable<Post>("select * from Posts").ToList();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return db.SqlQueryable<dynamic>($"select * from Posts where Id = {Id}").Single();
        }

        public IPost GetItemAsObject(int Id)
        {
            return db.SqlQueryable<Post>($"select * from Posts where Id = {Id}").Single();
        }

        public void Dispose()
        {
            db.Close();
        }

        private readonly DatabaseProvider[] Supported = new[]
     {
           DatabaseProvider.MySqlData,
           DatabaseProvider.SystemData,
           DatabaseProvider.Npgsql
        };

        public bool IsSupported(DatabaseProvider databaseType) => Supported.Contains(databaseType);
    }
}