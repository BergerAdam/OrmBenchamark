using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using OrmBenchmark.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OrmBenchmark.DevExpress
{
    public class DevExpressQueryExecuter : IOrmExecuter
    {
        private UnitOfWork uow;
        public DatabaseProvider DatabaseProvider { get; private set; }

        public string Name
        {
            get
            {
                return "DevExpress Xpo";
            }
        }

        public void Init(string connectionString, DatabaseProvider databaseType)
        {
            DatabaseProvider = databaseType;
            if(DatabaseProvider == DatabaseProvider.MicrosoftData || DatabaseProvider == DatabaseProvider.SystemData)
            {
               
                XpoDefault.DataLayer = new SimpleDataLayer(new MSSqlConnectionProvider(DatabaseProvider.GetConnection(connectionString), AutoCreateOption.SchemaAlreadyExists));

            }
            else
            {
                XpoDefault.DataLayer = XpoDefault.GetDataLayer(CreateConnectionString(connectionString, databaseType), AutoCreateOption.SchemaAlreadyExists);

            }
            uow = new UnitOfWork();

        }

        private string CreateConnectionString(string connectionString, DatabaseProvider databaseType)
        {
            string xpoProvider;
            switch (databaseType)
            {
                case DatabaseProvider.MySqlData:
                    xpoProvider = "MySql";
                    break;

                case DatabaseProvider.Npgsql:
                    xpoProvider = "Postgres";
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
            if (XpoDefault.DataLayer.Connection.State == ConnectionState.Open)
            {
                XpoDefault.DataLayer.Connection.Close();
            }
            uow.Dispose();
        }

        private readonly DatabaseProvider[] Supported = new[]
        {
           DatabaseProvider.MySqlData,
           DatabaseProvider.SystemData,
           DatabaseProvider.MicrosoftData,
           DatabaseProvider.Npgsql
        };

        public bool IsSupported(DatabaseProvider databaseType) => Supported.Contains(databaseType);
    }
}