using Microsoft.EntityFrameworkCore;
using OrmBenchmark.Core;
using System;

namespace OrmBenchmark.EntityFramework
{
    internal class OrmBenchmarkContext : DbContext
    {
        private string ConnectionString;
        private readonly DatabaseProvider _databaseProvider;

        public OrmBenchmarkContext(string connectionStrong, DatabaseProvider databaseProvider)
        {
            ConnectionString = connectionStrong;
            _databaseProvider = databaseProvider;
        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_databaseProvider)
            {
                case DatabaseProvider.MySqlConnector:
                    MySqlDbContextOptionsBuilderExtensions.UseMySql(optionsBuilder, ConnectionString, new MySqlServerVersion(new Version(5, 7, 32)));
                    break;

                // TODO: Not working
                //case DatabaseProvider.MySqlData: 
                //    MySQLDbContextOptionsExtensions.UseMySQL(optionsBuilder,ConnectionString);
                //    break;

                case DatabaseProvider.Npgsql:
                    optionsBuilder.UseNpgsql(ConnectionString);

                    break;

                case DatabaseProvider.SystemData:
                case DatabaseProvider.MicrosoftData:
                    optionsBuilder.UseSqlServer(_databaseProvider.GetConnection(ConnectionString));
                    break;

                default:
                    break;
            }
        }
    }
}