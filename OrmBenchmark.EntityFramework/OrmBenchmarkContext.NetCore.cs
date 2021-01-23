using Microsoft.EntityFrameworkCore;
using OrmBenchmark.Core;
using System;

namespace OrmBenchmark.EntityFramework
{
    internal class OrmBenchmarkContext : DbContext
    {
        private string ConnectionString;
        private readonly DatabaseType _databaseType;

        public OrmBenchmarkContext(string connectionStrong, DatabaseType databaseType)
        {
            ConnectionString = connectionStrong;
            _databaseType = databaseType;
        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_databaseType)
            {
                case DatabaseType.MySqlConnector:
                    optionsBuilder.UseMySql(ConnectionString, new MySqlServerVersion(new Version(5, 7, 32)));

                    break;

                case DatabaseType.PostgreSql:
                    optionsBuilder.UseNpgsql(ConnectionString);

                    break;

                case DatabaseType.SqlServer:
                    optionsBuilder.UseSqlServer(ConnectionString);
                    break;

                default:
                    break;
            }
        }
    }
}