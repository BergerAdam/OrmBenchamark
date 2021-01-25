using System;
using System.Collections.Generic;

namespace OrmBenchmark.Core
{
    public interface IOrmExecuter : IDisposable
    {
        string Name { get; }

        bool IsSupported(DatabaseProvider databaseType);

        DatabaseProvider DatabaseProvider { get; }

        void Init(string connectionString, DatabaseProvider databaseType);

        IPost GetItemAsObject(int Id);

        dynamic GetItemAsDynamic(int Id);

        IEnumerable<IPost> GetAllItemsAsObject();

        IEnumerable<dynamic> GetAllItemsAsDynamic();
    }

    public interface IOrmWithCacheExecuter : IOrmExecuter
    {
        void ClearCache();
    }
}