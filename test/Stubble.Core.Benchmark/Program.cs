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
            var config = (ManualConfig)ManualConfig
                .Create(DefaultConfig.Instance)
                .With(ExecutionValidator.FailOnError);

            config.Add(new MemoryDiagnoser());
            config.Add(new TagColumn("Renderer", name => name.Split('_')[0]));

            var summary = BenchmarkRunner.Run<Benchmarks>(config);
        }
    }
}
