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

        private readonly DatabaseProvider[] Supported = new[]
     {
           DatabaseProvider.MySqlData,
           DatabaseProvider.SystemData,
           DatabaseProvider.Npgsql
        };

        public bool IsSupported(DatabaseProvider databaseType) => Supported.Contains(databaseType);
    }
}