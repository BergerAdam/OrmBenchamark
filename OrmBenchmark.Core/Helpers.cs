using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data.Common;

namespace OrmBenchmark.Core
{
    public static class Helpers
    {
        public static string GetProviderName(this DatabaseProvider databaseType)
        {
            switch (databaseType)
            {
                case DatabaseProvider.MySqlData:
                    return "MySql.Data.MySqlClient";

                case DatabaseProvider.Npgsql:
                    return "Npgsql";

                case DatabaseProvider.SystemData:
                    return "System.Data.SqlClient";

                case DatabaseProvider.MicrosoftData:
                    return "Microsoft.Data.SqlClient";

                case DatabaseProvider.MySqlConnector:
                    return "MySqlConnector";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DbConnection GetConnection(this DatabaseProvider databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case DatabaseProvider.MySqlData:
                    return new MySqlConnection(connectionString);

                case DatabaseProvider.MySqlConnector:
                    return new MySqlConnector.MySqlConnection(connectionString);

                case DatabaseProvider.Npgsql:
                    return new NpgsqlConnection(connectionString);

                case DatabaseProvider.SystemData:
                    return new System.Data.SqlClient.SqlConnection(connectionString);

                case DatabaseProvider.MicrosoftData:
                    return new Microsoft.Data.SqlClient.SqlConnection(connectionString);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DbConnection GetOpenedConnection(this DatabaseProvider databaseType, string connectionString)
        {
            var conenction = databaseType.GetConnection(connectionString);
            conenction.Open();
            return conenction;
        }

        public static T GetAndConfigureConnection<T>(this DatabaseProvider databaseType, string connectionString, Func<DbConnection, T> configure)
        {
            var connection = databaseType.GetConnection(connectionString);
            return configure(connection);
        }

        public static T GetAndConfigureConnection<T>(this DatabaseProvider databaseType, string connectionString, Func<DbConnection, DatabaseProvider, T> configure)
        {
            var connection = databaseType.GetConnection(connectionString);
            return configure(connection, databaseType);
        }
    }
}