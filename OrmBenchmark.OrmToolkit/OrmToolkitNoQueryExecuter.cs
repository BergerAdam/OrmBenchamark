using OrmBenchmark.Core;
using ORMToolkit.Core;
using ORMToolkit.Core.CacheProvider;
using ORMToolkit.Core.Factories;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace OrmBenchmark.OrmToolkit
{
    public class OrmToolkitNoQueryExecuter : IOrmWithCacheExecuter
    {
        private DbConnection conn;

        public string Name
        {
            get
            {
                return "OrmToolkit (No Query)";
            }
        }

        public DatabaseType DatabaseType { get; private set; }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            conn = databaseType.GetOpenedConnection(connectionString);
        }

        public IPost GetItemAsObject(int Id)
        {
            return conn.Get<Post>(new { Id });
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return conn.Get("Posts", new { Id });
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.GetAll<Post>().ToList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.GetAll("Posts").ToList();
        }

        public void Dispose()
        {
            conn.Close();
        }

        public void ClearCache()
        {
            OrmToolkitSettings.ObjectFactory = new ObjectFactory2();
            OrmToolkitSettings.CommandsCache = new HashsetInstanceCache();
            OrmToolkitSettings.TypesCache = new HashsetInstanceCache();
        }

        public bool IsSupported(DatabaseType databaseType) => true;
    }
}