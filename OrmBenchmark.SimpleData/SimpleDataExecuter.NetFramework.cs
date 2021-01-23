using OrmBenchmark.Core;
using Simple.Data;
using System.Collections.Generic;

namespace OrmBenchmark.SimpleData
{
    public class SimpleDataExecuter : IOrmExecuter
    {
        dynamic sdb;

        public string Name
        {
            get
            {
                return "Simple.Data";
            }
        }

        public DatabaseType DatabaseType { get; set; }



        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            sdb = Database.OpenConnection(connectionString);
        }

        public IPost GetItemAsObject(int Id)
        {
            return null;
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return sdb.Posts.FindById(Id);
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return null;
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return sdb.Posts.All().ToList();
        }

        public void Dispose()
        {
           
        }
    }
}
