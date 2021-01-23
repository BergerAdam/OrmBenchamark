using OrmBenchmark.Core;
using ORMToolkit.Core;
using ORMToolkit.Core.CacheProvider;
using ORMToolkit.Core.Factories;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace OrmBenchmark.OrmToolkit
{
    public class OrmToolkitTestExecuter : IOrmWithCacheExecuter
    {
        private DbConnection conn;

        public string Name
        {
            get
            {
                return "OrmToolkit (Beta)";
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
            object param = new { Id = Id };
            return conn.Query<Post>("select * from Posts where Id=@Id", new { Id }).Single();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return conn.Query("select * from Posts where Id=@Id", new { Id }).First();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.Query<Post>("select * from Posts").ToList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.Query("select * from Posts").ToList();
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