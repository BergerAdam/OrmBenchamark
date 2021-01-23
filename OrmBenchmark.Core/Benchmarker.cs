using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace OrmBenchmark.Core
{
    public class Benchmarker
    {
        private List<Type> Executers { get; set; }
        public List<BenchmarkResult> Results { get; set; }
        public List<BenchmarkResult> ResultsForAllItems { get; set; }
        public List<BenchmarkResult> ResultsForDynamicItem { get; set; }
        public List<BenchmarkResult> ResultsForAllDynamicItems { get; set; }
        public List<BenchmarkResult> ResultsWarmUp { get; set; }
        private int IterationCount { get; set; }
        private Random Random { get; set; }
        private Dictionary<DatabaseType, string> ConnectionStrings { get; }
        private readonly IReadOnlyList<int> _idList;

        public Benchmarker(Dictionary<DatabaseType, string> connectionStrings, int iterationCount)
        {
            Random = new Random();
            ConnectionStrings = connectionStrings;
            IterationCount = iterationCount;
            Executers = new List<Type>();
            Results = new List<BenchmarkResult>();
            ResultsForDynamicItem = new List<BenchmarkResult>();
            ResultsForAllItems = new List<BenchmarkResult>();
            ResultsForAllDynamicItems = new List<BenchmarkResult>();
            ResultsWarmUp = new List<BenchmarkResult>();
            _idList = Enumerable.Range(1, 5000)
                               .OrderBy(i => Random.Next()).Take(iterationCount).ToList();

        }

        public void RegisterOrmExecuter<TOrmExecuter>() where TOrmExecuter : IOrmExecuter
        {
            Executers.Add(typeof(TOrmExecuter));
        }

        public void Run(bool warmUp = false)
        {

            Results.Clear();
            ResultsForDynamicItem.Clear();
            ResultsForAllItems.Clear();
            ResultsForAllDynamicItems.Clear();
            ResultsWarmUp.Clear();

            foreach (var database in ConnectionStrings.Keys)
            {
                PrepareDatabase(database);

                foreach (Type executerType in Executers.OrderBy(ignore => Random.Next()))
                {
                    var executer = Activator.CreateInstance(executerType) as IOrmExecuter;
                    if (!executer.IsSupported(database))
                    {
                        continue;
                    }

                    using (executer)
                    {
                       
                        executer.Init(ConnectionStrings[database], database);

                        // Warm-up

                        if (warmUp)
                        {
                            WarmUp(executer);
                        }

                        // Object
                        GetItemAsObjectTest(executer);

                        // Dynamic
                        GetItemAsDynamicTest(executer);

                        // All Objects
                        GetAllItemsAsObjectTest(executer);

                        // All Dynamics
                        GetAllItemsAsDynamicTest(executer);
                    }
                }
            }
        }

        private void GetAllItemsAsDynamicTest(IOrmExecuter executer)
        {
            if (executer is IOrmWithCacheExecuter ormWithCacheExecuter)
            {
                ormWithCacheExecuter.ClearCache();
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = executer.GetAllItemsAsDynamic();
            stopwatch.Stop();

            if (result is null)
            {
                return;
            }

            if (!result.Any())
            {
                return;
            }

            ResultsForAllDynamicItems.Add(CreateResult(executer, GetNanoSeconds(stopwatch), nameof(GetAllItemsAsDynamicTest)));
        }

        private void GetAllItemsAsObjectTest(IOrmExecuter executer)
        {
            if (executer is IOrmWithCacheExecuter ormWithCacheExecuter)
            {
                ormWithCacheExecuter.ClearCache();
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = executer.GetAllItemsAsObject();
            stopwatch.Stop();

            if (result is null)
            {
                return;
            }

            if (!result.Any())
            {
                return;
            }

            ResultsForAllItems.Add(CreateResult(executer, GetNanoSeconds(stopwatch), nameof(GetAllItemsAsObjectTest)));
        }


        private static double GetNanoSeconds(Stopwatch stopwatch)
        {
            double ticks = stopwatch.ElapsedTicks;
            long frequency = Stopwatch.Frequency;
            return ticks / frequency * (1000L * 1000L * 1000L);
        }

        private void GetItemAsDynamicTest(IOrmExecuter executer)
        {
            if (executer is IOrmWithCacheExecuter ormWithCacheExecuter)
            {
                ormWithCacheExecuter.ClearCache();
            }

            double firstItemExecTime = 0;
            double executionTime = 0;
            bool onlyFirst = true;
            foreach (var id in _idList)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var result = executer.GetItemAsDynamic(id);
                stopwatch.Stop();

                if (result is null)
                {
                    return;
                }

                executionTime += GetNanoSeconds(stopwatch);
                if (onlyFirst)
                {
                    firstItemExecTime = executionTime;
                    onlyFirst = false;
                }
            }
            ResultsForDynamicItem.Add(CreateResult(executer, executionTime, nameof(GetItemAsDynamicTest), firstItemExecTime));
        }

        private void GetItemAsObjectTest(IOrmExecuter executer)
        {
            if (executer is IOrmWithCacheExecuter ormWithCacheExecuter)
            {
                ormWithCacheExecuter.ClearCache();
            }

            double executionTime = 0;
            double firstItemExecTime = 0;
            bool onlyFirst = true;
            foreach (var id in _idList)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var result = executer.GetItemAsObject(id); ;
                stopwatch.Stop();

                if (result is null)
                {
                    return;
                }

                executionTime += GetNanoSeconds(stopwatch);

                if (onlyFirst)
                {
                    firstItemExecTime = executionTime;
                    onlyFirst = false;
                }
            }
            Results.Add(CreateResult(executer, executionTime, nameof(GetItemAsObjectTest), firstItemExecTime));
        }

        private void WarmUp(IOrmExecuter executer)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            executer.GetItemAsObject(1);
            executer.GetItemAsDynamic(1);
            stopwatch.Stop();

            ResultsWarmUp.Add(CreateResult(executer, GetNanoSeconds(stopwatch), "WarmUp"));
        }

        private static BenchmarkResult CreateResult(IOrmExecuter executer, double elapsedMilliseconds, string testName, double? firstItemExecTime = null)
        {
            return new BenchmarkResult
            {
                Name = executer.Name,
                DatabaseType = executer.DatabaseType.ToString(),
                ExecTime = elapsedMilliseconds,
                FirstItemExecTime = firstItemExecTime,
                TestName = testName
            };
        }

        private void PrepareDatabase(DatabaseType databaseType)
        {

            switch (databaseType)
            {
                case DatabaseType.MySql:
                case DatabaseType.MySqlConnector:
                    PrepareMySql();
                    break;
                case DatabaseType.PostgreSql:
                    PreparePostgreSql();
                    break;
                case DatabaseType.SqlServer:
                    PrepareSqlServer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PrepareMySql()
        {
            using (var conn = new MySqlConnection(ConnectionStrings[DatabaseType.MySql]))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
DROP Procedure IF EXISTS CreatePosts;

CREATE procedure CreatePosts () 
DETERMINISTIC
BEGIN 
 DECLARE i int DEFAULT 0;
 IF NOT EXISTS(
       SELECT * FROM information_schema.tables 
       Where table_name = 'Posts'
)
 THEN 
 
 create table Posts
	                    (
		                    Id int primary key AUTO_INCREMENT, 
		                    Text varchar(2000) not null, 
		                    CreationDate datetime not null, 
		                    LastChangeDate datetime not null,
		                    Counter1 int,
		                    Counter2 int,
		                    Counter3 int,
		                    Counter4 int,
		                    Counter5 int,
		                    Counter6 int,
		                    Counter7 int,
		                    Counter8 int,
		                    Counter9 int
	                    );
                        
    
    WHILE i <= 5001 DO
        INSERT INTO Posts (Text, CreationDate,LastChangeDate) VALUES (REPEAT('x',2000) , NOW(),NOW());
        SET i = i + 1;
    END WHILE;
 
 
END IF;

END
";

                cmd.Connection = conn;
                cmd.ExecuteNonQuery();

                var cmd2 = conn.CreateCommand();
                cmd2.CommandText = "CreatePosts";
                cmd2.Connection = conn;
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.ExecuteNonQuery();
                conn.Close();

            }
        }

        private void PreparePostgreSql()
        {
            using (var conn = new NpgsqlConnection(ConnectionStrings[DatabaseType.PostgreSql]))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
CREATE OR REPLACE FUNCTION create_mytable()
  RETURNS void
  LANGUAGE plpgsql AS
$func$
BEGIN
   IF EXISTS (SELECT FROM pg_catalog.pg_tables 
             where   tablename  = 'posts') THEN
      return;
   ELSE
      create table Posts
	                    (
		                    Id int primary key GENERATED ALWAYS AS IDENTITY , 
		                    Text Text not null, 
		                    CreationDate timestamp  not null, 
		                    LastChangeDate timestamp  not null,
		                    Counter1 int,
		                    Counter2 int,
		                    Counter3 int,
		                    Counter4 int,
		                    Counter5 int,
		                    Counter6 int,
		                    Counter7 int,
		                    Counter8 int,
		                    Counter9 int
	                    );

do $$
begin
for r in 1..5001 loop
 INSERT INTO Posts (Text,CreationDate,LastChangeDate)
 Values 			(REPEAT('x', 2000),now(),now());
end loop;
end;
$$;
    
   END IF;
END
$func$;

select create_mytable()";
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void PrepareSqlServer()
        {
            using (var conn = new SqlConnection(ConnectionStrings[DatabaseType.SqlServer]))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    if (OBJECT_ID('Posts') is null)
                    begin
	                    create table Posts
	                    (
		                    Id int identity primary key, 
		                    [Text] varchar(max) not null, 
		                    CreationDate datetime not null, 
		                    LastChangeDate datetime not null,
		                    Counter1 int,
		                    Counter2 int,
		                    Counter3 int,
		                    Counter4 int,
		                    Counter5 int,
		                    Counter6 int,
		                    Counter7 int,
		                    Counter8 int,
		                    Counter9 int
	                    )
	   
	                    set nocount on 

	                    declare @i int
	                    declare @c int
	                    declare @id int
	                    set @i = 0

	                    while @i <= 5001
	                    begin 
		                    insert Posts ([Text], CreationDate, LastChangeDate) values (replicate('x', 2000), GETDATE(), GETDATE())
		                    set @id = @@IDENTITY
		
		                    set @i = @i + 1
	                    end
                    end";

                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
