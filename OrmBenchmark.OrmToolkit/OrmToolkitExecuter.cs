﻿using OrmBenchmark.Core;
using System.Collections.Generic;
using ORMToolkit.Core;
using ORMToolkit.Core.Factories;
using ORMToolkit.Core.CacheProvider;
using System.Linq;
using System.Data.Common;

namespace OrmBenchmark.OrmToolkit
{
    public class OrmToolkitExecuter : IOrmWithCacheExecuter
    {
        DbConnection conn;

        public DatabaseType DatabaseType { get; private set; }
        public string Name
        {
            get
            {
                return "OrmToolkit";
            }
        }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            conn = databaseType.GetOpenedConnection(connectionString);
        }

        public IPost GetItemAsObject(int Id)
        {
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