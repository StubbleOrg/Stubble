using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using Stubble.Core.Classes;
using Stubble.Core.Dev.Parser;
using BenchmarkDotNet.Diagnosers;
using System.Linq;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters;

namespace Stubble.Core.Benchmark
{
    [Config(typeof(Config))]
    public class ParserBenchmarks
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                Add(new MemoryDiagnoser());
                Add(ExecutionValidator.FailOnError);
                Add(CsvMeasurementsExporter.Default);
                Add(RPlotExporter.Default);
            }
        }

        public const string template = "{{#foo}}\r\n  {{#a}}\r\n    {{b}}\r\n  {{/a}}\r\n{{/foo}}\r\n";

        [Benchmark(Baseline = true)]
        public void RegexParser()
        {
            var registry = new Registry();
            new Parser(registry.TokenMatchRegex, registry.TokenGetters).ParseTemplate(template);
        }

        [Benchmark]
        public void NonRegexParser()
        {
            MustacheParser.Parse(template);
        }

        [Benchmark]
        public void NustacheParser()
        {
            new Nustache.Core.Scanner().Scan(template).ToList();
        }
    }
}
