using Microsoft.EntityFrameworkCore;
using OrmBenchmark.Core;
using System.Collections.Generic;
using System.Linq;

namespace OrmBenchmark.EntityFramework
{
    public class EntityFrameworkNoTrackingExecuter : IOrmExecuter
    {
        OrmBenchmarkContext ctx;
        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "Entity Framework (NoTracking)";
            }
        }

        public void Init(string connectionStrong, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            ctx = new OrmBenchmarkContext(connectionStrong, DatabaseType);
        }

        public IPost GetItemAsObject(int Id)
        {
            return ctx.Posts.AsNoTracking().Single(p => p.Id == Id);

        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return null;
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return ctx.Posts.AsNoTracking().ToList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return null;
        }

        public void Dispose()
        {
            ctx.Dispose();
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
