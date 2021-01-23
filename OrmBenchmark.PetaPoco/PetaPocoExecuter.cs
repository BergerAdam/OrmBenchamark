﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrmBenchmark.Core;
using PetaPoco;

namespace OrmBenchmark.PetaPoco
{
    public class PetaPocoExecuter : IOrmExecuter
    {
        Database petapoco;
        public DatabaseType DatabaseType { get; private set; }
        public string Name
        {
            get
            {
                return "PetaPoco";
            }
        }

        public void Init(string connectionStrong, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            petapoco = new Database(connectionStrong, databaseType.GetProviderName());
            petapoco.OpenSharedConnection();
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