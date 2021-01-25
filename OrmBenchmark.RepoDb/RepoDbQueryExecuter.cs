using OrmBenchmark.Core;
using RepoDb;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Extensions;
using RepoDb.StatementBuilders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace OrmBenchmark.RepoDb
{
    public class RepoDbQueryExecuter : IOrmExecuter
    {
        private DbConnection conn;

        public DatabaseProvider DatabaseProvider { get; private set; }

        public string Name
        {
            get
            {
                return "RepoDb Query";
            }
        }

        public void Init(string connectionString, DatabaseProvider databaseType)
        {
            DatabaseProvider = databaseType;
            conn = DatabaseProvider.GetAndConfigureConnection(connectionString, (connection, dbType) =>
            {
                switch (dbType)
                {
                    case DatabaseProvider.MySqlData:
                        MySqlBootstrap.Initialize();
                        break;

                    case DatabaseProvider.Npgsql:
                        PostgreSqlBootstrap.Initialize();
                        break;

                    case DatabaseProvider.SystemData:
                        {
                            var dbSetting = new SqlServerDbSetting();
                            DbSettingMapper.Add(typeof(System.Data.SqlClient.SqlConnection), dbSetting, true);

                            // Map the DbHelper
                            var dbHelper = new SqlServerDbHelper();
                            DbHelperMapper.Add(typeof(System.Data.SqlClient.SqlConnection), dbHelper, true);

                            // Map the Statement Builder
                            var statementBuilder = new SqlServerStatementBuilder(dbSetting);
                            StatementBuilderMapper.Add(typeof(System.Data.SqlClient.SqlConnection), statementBuilder, true);
                            break;
                        }
                    case DatabaseProvider.MicrosoftData:
                        {
                            var dbSetting = new SqlServerDbSetting();
                            DbSettingMapper.Add(typeof(Microsoft.Data.SqlClient.SqlConnection), dbSetting, true);

                            // Map the DbHelper
                            var dbHelper = new SqlServerDbHelper();
                            DbHelperMapper.Add(typeof(Microsoft.Data.SqlClient.SqlConnection), dbHelper, true);

                            // Map the Statement Builder
                            var statementBuilder = new SqlServerStatementBuilder(dbSetting);
                            StatementBuilderMapper.Add(typeof(Microsoft.Data.SqlClient.SqlConnection), statementBuilder, true);
                            break;
                        }

                    case DatabaseProvider.MySqlConnector:
                        MySqlConnectorBootstrap.Initialize();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                connection.Open();
                return connection;
            });
        }

        public void Dispose()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            CommandTextCache.Flush();

            conn.Dispose();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.QueryAll<Post>().AsList();
        }

        public IPost GetItemAsObject(int Id)
        {
            return conn.Query<Post>(Id).Single();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.QueryAll("Posts").AsList(); ;
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            return conn.Query("Posts", Id).Single();
        }

        public bool IsSupported(DatabaseProvider databaseType) => true;
    }
}