using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using OrmBenchmark.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OrmBenchmark.DevExpress
{
    public class DevExpressQueryExecuter : IOrmExecuter
    {
        UnitOfWork uow;
        public DatabaseType DatabaseType { get;private set; }


        

        public string Name
        {
            get
            {
                return "DevExpress Xpo";
            }
        }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(CreateConnectionString(connectionString,databaseType), AutoCreateOption.DatabaseAndSchema);

            uow = new UnitOfWork();
        }

        private string CreateConnectionString(string connectionString, DatabaseType databaseType)
        {
            string xpoProvider;
            switch (databaseType)
            {
                case DatabaseType.MySql:
                    xpoProvider = "MySql";
                    break;
                case DatabaseType.PostgreSql:
                    xpoProvider = "Postgres";
                    break;
                case DatabaseType.SqlServer:
                    xpoProvider = "MSSqlServer";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return $"XpoProvider={xpoProvider};{connectionString}";
        }

        public IPost GetItemAsObject(int Id)
        {
            return uow.Query<Post>().Where(i => i.Id == Id).Single();

        }

        public dynamic GetItemAsDynamic(int Id)
        {
            var q = uow.Query<Post>()
                .Where(i => i.Id == Id)
                .Select(p => new
                {
                    p.Id,
                    p.Text,
                    p.CreationDate,
                    p.LastChangeDate,
                    p.Counter1,
                    p.Counter2,
                    p.Counter3,
                    p.Counter4,
                    p.Counter5,
                    p.Counter6,
                    p.Counter7,
                    p.Counter8,
                    p.Counter9
                });

            return q.Single();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return uow.Query<Post>().ToList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            var q = uow.Query<Post>()
                .Select(p => new
                {
                    p.Id,
                    p.Text,
                    p.CreationDate,
                    p.LastChangeDate,
                    p.Counter1,
                    p.Counter2,
                    p.Counter3,
                    p.Counter4,
                    p.Counter5,
                    p.Counter6,
                    p.Counter7,
                    p.Counter8,
                    p.Counter9
                });

            return q.ToList();
        }
        public void Dispose()
        {
            if(XpoDefault.DataLayer.Connection.State == ConnectionState.Open)
            {
                XpoDefault.DataLayer.Connection.Close();
            }
            uow.Dispose();
        }

        private readonly DatabaseType[] Supported = new[]
        {
           DatabaseType.MySql,
           DatabaseType.SqlServer,
           DatabaseType.PostgreSql
        };

        public bool IsSupported(DatabaseType databaseType) => Supported.Contains(databaseType);
    }
}
