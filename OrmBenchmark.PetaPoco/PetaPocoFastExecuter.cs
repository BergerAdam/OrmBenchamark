using OrmBenchmark.Core;
using PetaPoco;
using System.Collections.Generic;
using System.Linq;

namespace OrmBenchmark.PetaPoco
{
    public class PetaPocoFastExecuter : IOrmExecuter
    {
        private Database petapoco;
        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "PetaPoco (Fast)";
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
            return petapoco.Query<Post>("select * from Posts where Id=@0", Id).Single();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return petapoco.Fetch<dynamic>("select * from Posts where Id=@0", Id).Single();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return petapoco.Query<Post>("select * from Posts").ToList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return petapoco.Query<dynamic>("select * from Posts").ToList();
        }

        public void Dispose()
        {
            petapoco.CloseSharedConnection();
            petapoco.Dispose();
        }

        public bool IsSupported(DatabaseType databaseType) => true;
    }
}