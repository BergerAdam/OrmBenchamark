using OrmBenchmark.Core;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OrmBenchmark.OrmLite
{
    public class OrmLiteExecuter : IOrmExecuter
    {
        private IDbConnection conn;
        private OrmLiteConnectionFactory dbFactory;
        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "Orm Lite";
            }
        }

        public void Init(string connectionStrong, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            dbFactory = new OrmLiteConnectionFactory(connectionStrong, SelectDialect());
            conn = dbFactory.Open();
        }

        private IOrmLiteDialectProvider SelectDialect()
        {
            switch (DatabaseType)
            {
                case DatabaseType.MySql:
                    return MySqlDialect.Provider;

                case DatabaseType.MySqlConnector:
                    return MySqlConnectorDialect.Provider;

                case DatabaseType.PostgreSql:
                    return PostgreSqlDialect.Provider;

                case DatabaseType.SqlServer:
                    return SqlServerDialect.Provider;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IPost GetItemAsObject(int Id)
        {
            return conn.Single<Post>("select * from Posts where Id=@Id", new { Id });
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return conn.Single<dynamic>("select * from Posts where Id=@Id", new { Id });
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.Select<Post>("select * from Posts");
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.Select<dynamic>("select * from Posts");
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