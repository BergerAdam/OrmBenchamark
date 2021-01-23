using System.Collections.Generic;
using System.Linq;
using OrmBenchmark.Core;
using PetaPoco;


namespace OrmBenchmark.PetaPoco
{
    public class PetaPocoFetchFastExecuter : IOrmExecuter
    {
        Database petapoco;
        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "PetaPoco (Fetch Fast)";
            }
        }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            petapoco = new Database(connectionString, DatabaseType.GetProviderName());
            petapoco.OpenSharedConnection();
            petapoco.EnableAutoSelect = false;
            petapoco.EnableNamedParams = false;
        }

        public IPost GetItemAsObject(int Id)
        {
            object param = new { Id = Id };
            return petapoco.Fetch<Post>("select * from Posts where Id=@0", Id).Single();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            object param = new { Id = Id };
            return petapoco.Fetch<dynamic>("select * from Posts where Id=@0", Id).Single();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return petapoco.Fetch<Post>("select * from Posts").ToList();
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
        public bool IsSupported(DatabaseType databaseType) => true;


    }
}
