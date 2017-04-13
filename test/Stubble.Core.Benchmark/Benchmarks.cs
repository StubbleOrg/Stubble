using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Nustache.Core;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters;
using Stubble.Core.Dev;

namespace Stubble.Core.Benchmark
{
    [Config(typeof(Config))]
    public class Benchmarks
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                Add(new MemoryDiagnoser());
                Add(ExecutionValidator.FailOnError);
                Add(new TagColumn("Renderer", name => name.Split('_')[0]));
                Add(CsvMeasurementsExporter.Default);
                Add(RPlotExporter.Default);
            }
        }

        [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13)]
        public int Index { get; set; }

        private StubbleStringRenderer Stubble;
        private StubbleVisitorRenderer StubbleVisitorRenderer;

        [Setup]
        public void SetupRenderers()
        {
            Stubble = new StubbleStringRenderer();
            StubbleVisitorRenderer = new StubbleVisitorRenderer();
        }

        [Benchmark]
        public string Stubble_NoCache()
        {
            var testCase = TestCaseParams.Params[Index - 1];
            var str = Stubble.Render(testCase.Key, testCase.Value);
            Stubble.ClearCache();
            return str;
        }

        [Benchmark(Baseline = true)]
        public string Stubble_Benchmark()
        {
            var testCase = TestCaseParams.Params[Index - 1];
            return Stubble.Render(testCase.Key, testCase.Value);
        }

        [Benchmark]
        public string Stubble_VistorBenchmark()
        {
            var testCase = TestCaseParams.Params[Index - 1];
            return StubbleVisitorRenderer.Render(testCase.Key, testCase.Value);
        }

        [Benchmark]
        public string Nustache_Benchmark()
        {
            var testCase = TestCaseParams.Params[Index - 1];
            return Render.StringToString(testCase.Key, testCase.Value);
        }

    }

    public static class TestCaseParams
    {
        public static readonly KeyValuePair<string, object>[] Params = {
            new KeyValuePair<string, object>("{{Foo}}", new {Foo = "Bar"}),
            new KeyValuePair<string, object>("Just Literal Data", null),
            new KeyValuePair<string, object>("Inline Lambda {{Foo}}", new {Foo = new Func<object>(() => "Bar")}),
            new KeyValuePair<string, object>("Section Lambda {{#Foo}} Section Stuff {{/Foo}}",
                new {Foo = new Func<string, object>(str => "Bar")}),
            new KeyValuePair<string, object>(
                "Section Lambda {{#Foo}} Child Section {{#ChildSection}} Stuff! {{/ChildSection}} {{/Foo}}",
                new {Foo = new {ChildSection = true}}),
            new KeyValuePair<string, object>("{{Foo}} New Tags {{=<% %>=}} <% Bar %> ", new {Foo = "Bar", Bar = "Foo"}),
            new KeyValuePair<string, object>("{{Foo}} New Tags {{=<% %>=}} <% Bar %> <%=<| |>=%> <|Foo|>",
                new {Foo = "Bar", Bar = "Foo"}),
            new KeyValuePair<string, object>("{{Property}}", new StronglyTypedTestClass {Property = 1}),
            new KeyValuePair<string, object>("{{StaticField}}", new StronglyTypedTestClass()),
            new KeyValuePair<string, object>("{{StaticProperty}}", new StronglyTypedTestClass()),
            new KeyValuePair<string, object>("{{Field}}", new StronglyTypedTestClass {Field = 1}),
            new KeyValuePair<string, object>("{{StaticMethodWithNoArgs}}", new StronglyTypedTestClass()),
            new KeyValuePair<string, object>("{{^StaticMethodWithArgs}}Stuff{{/StaticMethodWithArgs}}", new StronglyTypedTestClass())
        };
    }

    public class StronglyTypedTestClass
    {
        #region Statics
        public static int StaticProperty
        {
            get;
            set;
        }
        public static int StaticField;
        public static int StaticMethodWithNoArgs()
        {
            return 1;
        }
        public static int StaticMethodWithArgs(int i)
        {
            return i;
        }
        #endregion
        #region Instance Variables
        public int Property { get; set; }
        public int Field;
        public int MethodWithoutArgs()
        {
            return 1;
        }
        public int MethodWithArgs(int i)
        {
            return i;
        }

        public event EventHandler Foo;

        #endregion

        protected virtual void OnFoo()
        {
            var handler = Foo;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }

    public class StronglyTypedChildTestClass : StronglyTypedTestClass
    {
        #region Statics
        public static int ChildStaticProperty { get; set; }
        public static int ChildStaticField;
        public static int ChildStaticMethodWithoutArgs()
        {
            return 2;
        }
        #endregion
        #region Instance Variables
        public int ChildProperty { get; set; }
        public int ChildField { get; set; }
        public int ChildMethodWithoutArgs()
        {
            return 2;
        }
        #endregion
    }
}
