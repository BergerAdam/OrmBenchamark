using Microsoft.EntityFrameworkCore;
using OrmBenchmark.Core;
using System.Collections.Generic;
using System.Linq;

namespace OrmBenchmark.EntityFramework
{
    public class EntityFrameworkNoTrackingExecuter : IOrmExecuter
    {
        private OrmBenchmarkContext ctx;
        public DatabaseProvider DatabaseProvider { get; private set; }

        public string Name
        {
            get
            {
                return "Entity Framework (NoTracking)";
            }
        }

        public void Init(string connectionStrong, DatabaseProvider databaseType)
        {
            DatabaseProvider = databaseType;
            ctx = new OrmBenchmarkContext(connectionStrong, DatabaseProvider);
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

        private readonly DatabaseProvider[] Supported = new[]
        {
           DatabaseProvider.MySqlConnector,
           DatabaseProvider.SystemData,
           DatabaseProvider.Npgsql
        };

        public bool IsSupported(DatabaseProvider databaseType) => Supported.Contains(databaseType);
    }
}