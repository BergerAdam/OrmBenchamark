using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace OrmBenchmark.Core
{
    public static class Helpers
    {
        public static string GetProviderName(this DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MySql:
                    return "MySql.Data.MySqlClient";

                case DatabaseType.PostgreSql:
                    return "Npgsql";

                case DatabaseType.SqlServer:
                    return "System.Data.SqlClient";

                case DatabaseType.MySqlConnector:
                    return "MySqlConnector";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DbConnection GetConnection(this DatabaseType databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case DatabaseType.MySql:
                    return new MySqlConnection(connectionString);

                case DatabaseType.MySqlConnector:
                    return new MySqlConnector.MySqlConnection(connectionString);

                case DatabaseType.PostgreSql:
                    return new NpgsqlConnection(connectionString);

                case DatabaseType.SqlServer:
                    return new SqlConnection(connectionString);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DbConnection GetOpenedConnection(this DatabaseType databaseType, string connectionString)
        {
            var conenction = databaseType.GetConnection(connectionString);
            conenction.Open();
            return conenction;
        }

        public static T GetAndConfigureConnection<T>(this DatabaseType databaseType, string connectionString, Func<DbConnection, T> configure)
        {
            var connection = databaseType.GetConnection(connectionString);
            return configure(connection);
        }

        public static T GetAndConfigureConnection<T>(this DatabaseType databaseType, string connectionString, Func<DbConnection, DatabaseType, T> configure)
        {
            var connection = databaseType.GetConnection(connectionString);
            return configure(connection, databaseType);
        }
    }
}