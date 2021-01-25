using OrmBenchmark.Core;
using System.Collections.Generic;
using System.Linq;

namespace OrmBenchmark.EntityFramework
{
    public class EntityFrameworkExecuter : IOrmWithCacheExecuter
    {
        private OrmBenchmarkContext ctx;
        public DatabaseProvider DatabaseProvider { get; private set; }

        public string Name
        {
            get
            {
                return "Entity Framework";
            }
        }

        public void Init(string connectionString, DatabaseProvider databaseType)
        {
            DatabaseProvider = databaseType;
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

        public bool IsSupported(DatabaseProvider databaseType) => true;
    }
}