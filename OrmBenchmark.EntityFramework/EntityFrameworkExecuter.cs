using Microsoft.EntityFrameworkCore;
using OrmBenchmark.Core;
using System.Collections.Generic;
using System.Linq;

namespace OrmBenchmark.EntityFramework
{
    public class EntityFrameworkExecuter : IOrmWithCacheExecuter
    {
        private OrmBenchmarkContext ctx;
        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "Entity Framework";
            }
        }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            ctx = new OrmBenchmarkContext(connectionString, databaseType);
        }

        public IPost GetItemAsObject(int Id)
        {
            return ctx.Posts.Single(e => e.Id == Id);
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return null;
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return ctx.Posts.ToList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return null;
        }

        public void Dispose()
        {
            ctx.Dispose();
        }

        public void ClearCache()
        {
            ctx.Posts.Local.Clear();
        }

        private readonly DatabaseType[] Supported = new[]
        {
           DatabaseType.MySqlConnector,
           DatabaseType.SqlServer,
           DatabaseType.PostgreSql
        };

        public bool IsSupported(DatabaseType databaseType) => Supported.Contains(databaseType);
    }
}