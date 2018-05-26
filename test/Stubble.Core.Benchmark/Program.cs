using BenchmarkDotNet.Running;

namespace Stubble.Core.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(Benchmarks),
                typeof(ParserBenchmarks),
                typeof(TwitterBenchmark),
            });

            switcher.Run();
        }
    }
}
