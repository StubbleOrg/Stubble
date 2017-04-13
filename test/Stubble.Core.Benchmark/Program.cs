using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

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
