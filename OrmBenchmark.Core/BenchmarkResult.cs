namespace OrmBenchmark.Core
{
    public class BenchmarkResult
    {
        public string Name { get; set; }
        public double? FirstItemExecTime { get; set; }
        public double ExecTime { get; set; }
        public string DatabaseType { get; set; }
        public string TestName { get; set; }

        public double ExecTimeMiliseconds => ExecTime / (1000 * 1000);
        public double? FirstItemExecTimeMiliseconds => FirstItemExecTime is null ? null : ExecTime / (1000 * 1000);
    }
}