using OrmBenchmark.Core;
using RepoDb;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Extensions;
using RepoDb.StatementBuilders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OrmBenchmark.RepoDb
{
    public class RepoDbExecuter : IOrmExecuter
    {
        private IDbConnection conn;

        public DatabaseProvider DatabaseProvider { get; private set; }

        public string Name
        {
            get
            {
                return "RepoDb";
            }
        }

        public void Init(string connectionString, DatabaseProvider databaseProvider)
        {
            DatabaseProvider = databaseProvider;
            conn = DatabaseProvider.GetAndConfigureConnection<IDbConnection>(connectionString, (connection, dbType) =>
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

            if (DatabaseProvider == DatabaseProvider.MicrosoftData)
            {
                DbSettingMapper.Remove<Microsoft.Data.SqlClient.SqlConnection>();
                DbHelperMapper.Remove<Microsoft.Data.SqlClient.SqlConnection>();
                StatementBuilderMapper.Remove<Microsoft.Data.SqlClient.SqlConnection>();
            }

            if (DatabaseProvider == DatabaseProvider.SystemData)
            {
                DbSettingMapper.Remove<System.Data.SqlClient.SqlConnection>();
                DbHelperMapper.Remove<System.Data.SqlClient.SqlConnection>();
                StatementBuilderMapper.Remove<System.Data.SqlClient.SqlConnection>();
            }


            CommandTextCache.Flush();

            conn.Dispose();
        }

        public IPost GetItemAsObject(int Id)
        {
            object param = new { Id = Id };
            return conn.ExecuteQuery<Post>("select * from Posts where Id=@Id", param).Single();
        }

        public dynamic GetItemAsDynamic(int Id)
        {
            object param = new { Id = Id };
            return conn.ExecuteQuery("select * from Posts where Id=@Id", param).Single();
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            return conn.ExecuteQuery<Post>("select * from Posts").AsList();
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            return conn.ExecuteQuery("select * from Posts").AsList();
        }

        public bool IsSupported(DatabaseProvider databaseType) => true;
    }
}