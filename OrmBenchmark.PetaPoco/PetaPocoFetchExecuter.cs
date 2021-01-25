using OrmBenchmark.Core;
using PetaPoco;
using System.Collections.Generic;
using System.Linq;

namespace OrmBenchmark.PetaPoco
{
    public class PetaPocoFetchExecuter : IOrmExecuter
    {
        private Database petapoco;
        public DatabaseProvider DatabaseProvider { get; private set; }

        public string Name
        {
            get
            {
                return "PetaPoco (Fetch)";
            }
        }

        public void Init(string connectionString, DatabaseProvider databaseType)
        {
            DatabaseProvider = databaseType;
            petapoco = new Database(connectionString, DatabaseProvider.GetProviderName());
            petapoco.OpenSharedConnection();
        }

        public IPost GetItemAsObject(int Id)
        {
            return petapoco.Fetch<Post>("select * from Posts where Id=@0", Id).Single();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return petapoco.Fetch<Post>("select * from Posts where Id=@0", Id).Single();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return petapoco.Fetch<Post>("select * from Posts");
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return petapoco.Fetch<dynamic>("select * from Posts");
        }

        public void Dispose()
        {
            petapoco.CloseSharedConnection();
            petapoco.Dispose();
        }

        public bool IsSupported(DatabaseProvider databaseType) => true;
    }
}