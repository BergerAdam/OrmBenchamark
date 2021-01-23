using System;
using System.Collections.Generic;
using System.Linq;
using OrmBenchmark.Core;
using SqlSugar;

namespace OrmBenchmark.SqlSugar
{
    public class SqlSugarExecuter : IOrmExecuter
    {
        public string Name => "SqlSugar";
        SqlSugarClient db;
        public DatabaseType DatabaseType { get;private  set; }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
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
            switch (DatabaseType)
            {
                case DatabaseType.PostgreSql:
                    return DbType.PostgreSQL;
                case DatabaseType.MySql:
                    return DbType.MySql;
                case DatabaseType.SqlServer:
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



        private readonly DatabaseType[] Supported = new[]
     {
           DatabaseType.MySql,
           DatabaseType.SqlServer,
           DatabaseType.PostgreSql
        };

        public bool IsSupported(DatabaseType databaseType) => Supported.Contains(databaseType);
    }
}
