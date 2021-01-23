using System;
using System.Collections.Generic;

namespace OrmBenchmark.Core
{
    public interface IOrmExecuter : IDisposable
    {
        string Name { get; }

        bool IsSupported(DatabaseType databaseType);

        DatabaseType DatabaseType { get; }
        void Init(string connectionString, DatabaseType databaseType);
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
