using OrmBenchmark.Core;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrmBenchmark.SqlSugar
{
    public class SqlSugarQueryableExecuter : IOrmExecuter
    {
        public string Name => "SqlSugar (Queryable)";
        private SqlSugarClient db;
        public DatabaseType DatabaseType { get; private set; }

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
            //return db.Queryable<dynamic>("Posts").ToList();
            return null;
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return db.Queryable<Post>().ToList();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            //return db.Queryable<dynamic>("Posts").InSingle(Id);
            return null;
        }

        public IPost GetItemAsObject(int Id)
        {
            return db.Queryable<Post>().InSingle(Id);
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