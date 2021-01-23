using Microsoft.Extensions.Configuration;
using OrmBenchmark.Ado;
using OrmBenchmark.Core;
using OrmBenchmark.Dapper;
using OrmBenchmark.DevExpress;
using OrmBenchmark.EntityFramework;
using OrmBenchmark.InsightDatabase;
using OrmBenchmark.NHibernate;
using OrmBenchmark.OrmLite;
using OrmBenchmark.OrmToolkit;
using OrmBenchmark.PetaPoco;
using OrmBenchmark.RepoDb;
using OrmBenchmark.SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace OrmBenchmark.ConsoleUI.NetCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            // Set up configuration sources.
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false).Build();
            Console.ForegroundColor = ConsoleColor.White;

            Dictionary<DatabaseType, string> connectionStrings =
            configuration.GetSection("ConnectionStrings").GetChildren().ToList().ToDictionary(e => Enum.Parse<DatabaseType>(e.Key), e => e.Value);
            bool warmUp = true;

            Console.WriteLine("ORM Benchmark");

            Console.WriteLine("Warm Up: " + warmUp);
            Console.WriteLine("Connection Strings");
            Dictionary<DatabaseType, string> connectionStateToBenchamrk = connectionStrings
                //.Where(e => e.Key == DatabaseType.MySql)
                .ToDictionary(e => e.Key, e => e.Value);

            foreach (var item in connectionStateToBenchamrk)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }

            var singleTestIterations = 500;
            var benchmarker = new Benchmarker(connectionStateToBenchamrk, singleTestIterations);

            benchmarker.RegisterOrmExecuter<PureAdoExecuter>();
            benchmarker.RegisterOrmExecuter<DapperBufferedExecuter>();
            benchmarker.RegisterOrmExecuter<DapperFirstOrDefaultExecuter>();
            benchmarker.RegisterOrmExecuter<DapperContribExecuter>();

            benchmarker.RegisterOrmExecuter<DevExpressQueryExecuter>();

            benchmarker.RegisterOrmExecuter<PetaPocoExecuter>();
            benchmarker.RegisterOrmExecuter<PetaPocoFastExecuter>();
            benchmarker.RegisterOrmExecuter<PetaPocoFetchExecuter>();
            benchmarker.RegisterOrmExecuter<PetaPocoFetchFastExecuter>();

            benchmarker.RegisterOrmExecuter<EntityFrameworkExecuter>();
            benchmarker.RegisterOrmExecuter<EntityFrameworkNoTrackingExecuter>();

            benchmarker.RegisterOrmExecuter<OrmLiteExecuter>();
            benchmarker.RegisterOrmExecuter<OrmLiteNoQueryExecuter>();

            benchmarker.RegisterOrmExecuter<RepoDbExecuter>();
            benchmarker.RegisterOrmExecuter<RepoDbQueryExecuter>();

            benchmarker.RegisterOrmExecuter<InsightDatabaseExecuter>();

            benchmarker.RegisterOrmExecuter<OrmToolkitExecuter>();
            benchmarker.RegisterOrmExecuter<OrmToolkitNoQueryExecuter>();
            benchmarker.RegisterOrmExecuter<OrmToolkitTestExecuter>();

            benchmarker.RegisterOrmExecuter<SqlSugarExecuter>();
            benchmarker.RegisterOrmExecuter<SqlSugarQueryableExecuter>();

            benchmarker.RegisterOrmExecuter<NHibernateExecutor>();
            benchmarker.RegisterOrmExecuter<NHibernateSqlQueryExecutor>();

            var ver = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
            Console.WriteLine(ver);

            try
            {
                benchmarker.Run(warmUp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex);
                LogError(ex);

                throw;
            }

            //                Console.ForegroundColor = ConsoleColor.Red;

            //if (warmUp)
            //{
            //    Console.WriteLine("\nPerformance of Warm-up:");
            //    ShowResults(benchmarker.ResultsWarmUp, false, false);
            //}

            //Console.WriteLine($"\nPerformance of select and map a row to a POCO object over {singleTestIterations} iterations:");
            //ShowResults(benchmarker.Results, true);

            //Console.WriteLine("\nPerformance of mapping 5000 rows to POCO objects in one iteration:");
            //ShowResults(benchmarker.ResultsForAllItems);

            //Console.WriteLine($"\nPerformance of select and map a row to a Dynamic object over {singleTestIterations} iterations:");
            //ShowResults(benchmarker.ResultsForDynamicItem, true);

            //Console.WriteLine("\nPerformance of mapping 5000 rows to Dynamic objects in one iteration:");
            //ShowResults(benchmarker.ResultsForAllDynamicItems);

            SaveResults(benchmarker, connectionStrings);

            stopWatch.Stop();
            Console.WriteLine($"Test take {stopWatch.ElapsedMilliseconds}");
        }

        private static void SaveResults(Benchmarker benchmarker, Dictionary<DatabaseType, string> connectionStrings)
        {
            if (!connectionStrings.TryGetValue(DatabaseType.SqlServer, out string connectionString))
            {
                return;
            }

            DateTime now = DateTime.Now;
            List<BenchmarkResult> resultsToAdd = new List<BenchmarkResult>();
            resultsToAdd.AddRange(benchmarker.ResultsWarmUp);
            resultsToAdd.AddRange(benchmarker.Results);
            resultsToAdd.AddRange(benchmarker.ResultsForDynamicItem);
            resultsToAdd.AddRange(benchmarker.ResultsForAllItems);
            resultsToAdd.AddRange(benchmarker.ResultsForAllDynamicItems);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
     @"
        INSERT INTO [dbo].[Results]
           ([Name]
           ,[ExecTime]
           ,[FirstItemExecTime]
           ,[Date]
           ,[DatabaseType]
           ,[TestName])
     VALUES
           (@Name,
           @ExecTime,
           @FirstItemExecTime,
           @Date,
           @DatabaseType,
           @TestName)
    ";

                    var Name = command.Parameters.Add("@Name", SqlDbType.NVarChar, 400);
                    var ExecTime = command.Parameters.Add("@ExecTime", SqlDbType.Decimal);
                    var FirstItemExecTime = command.Parameters.Add("@FirstItemExecTime", SqlDbType.Decimal);
                    var Date = command.Parameters.Add("@Date", SqlDbType.DateTime2);
                    var DatabaseType = command.Parameters.Add("@DatabaseType", SqlDbType.NVarChar, 400);
                    var TestName = command.Parameters.Add("@TestName", SqlDbType.NVarChar, 400);
                    command.Transaction = transaction;
                    foreach (var item in resultsToAdd)
                    {
                        Name.Value = item.Name;
                        ExecTime.Value = item.ExecTimeMiliseconds;
                        FirstItemExecTime.Value = item.FirstItemExecTime == null ? DBNull.Value : item.FirstItemExecTimeMiliseconds;
                        Date.Value = now;
                        DatabaseType.Value = item.DatabaseType;
                        TestName.Value = item.TestName;
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        private static void ShowResults(List<BenchmarkResult> results, bool showFirstRun = false, bool ignoreZeroTimes = true)
        {
            var defaultColor = Console.ForegroundColor;
            //Console.ForegroundColor = ConsoleColor.Gray;

            int i = 0;
            var list = results.OrderBy(o => o.ExecTime);
            if (ignoreZeroTimes)
                list = results.FindAll(o => o.ExecTime > 0).OrderBy(o => o.ExecTime);

            foreach (var result in list)
            {
                Console.ForegroundColor = i < 3 ? ConsoleColor.Green : ConsoleColor.Gray;

                if (showFirstRun)
                    Console.WriteLine(string.Format("{0,2}-{1,-40} {2,5} ms (First run: {3,3} ms)", ++i, result.Name + " - " + result.DatabaseType, result.ExecTimeMiliseconds, result.FirstItemExecTimeMiliseconds));
                else
                    Console.WriteLine(string.Format("{0,2}-{1,-40} {2,5} ms", ++i, result.Name + " - " + result.DatabaseType, result.ExecTimeMiliseconds));
            }

            Console.ForegroundColor = defaultColor;
        }

        private static void LogError(Exception ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = "ErrorLog.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }
}