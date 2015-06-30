using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nustache.Core;
using Stubble.Core.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Stubble.Core.Performance
{
    public class PerformanceTest
    {
        internal readonly ITestOutputHelper OutputStream;

        public PerformanceTest(ITestOutputHelper output)
        {
            OutputStream = output;
            StronglyTypedTestClass.StaticField = 1;
            StronglyTypedTestClass.StaticProperty = 1;
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(1000000)]
        public TimeSpan Simple_Template_Test(int iterations)
        {
            return Simple_Template_Test(iterations, false);
        }

        public TimeSpan Simple_Template_Test(int iterations, bool useCache)
        {
            var stopwatch = Stopwatch.StartNew();
            var stubble = new Stubble();

            for (var i = 0; i < iterations; i++)
            {
                var testCase = GetRenderTestCase(i);
                stubble.Render(testCase.Key, testCase.Value);
                if(!useCache) stubble.ClearCache();
            }
            stopwatch.Stop();

            OutputStream.WriteLine("Time Taken: {0} Milliseconds for {1:N0} iterations\nAverage {2} Ticks", 
                stopwatch.ElapsedMilliseconds,
                iterations,
                stopwatch.ElapsedTicks / (long)iterations);

            return stopwatch.Elapsed;
        }

        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(1000000)]
        public TimeSpan Simple_Template_Test_With_Cache(int iterations)
        {
            return Simple_Template_Test(iterations, true);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(1000000)]
        public TimeSpan Simple_Template_Test_Nustache(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < iterations; i++)
            {
                var testCase = GetRenderTestCase(i);
                Render.StringToString(testCase.Key, testCase.Value);
            }
            stopwatch.Stop();

            OutputStream.WriteLine("Time Taken: {0} Milliseconds for {1:N0} iterations\nAverage {2} Ticks",
                stopwatch.ElapsedMilliseconds,
                iterations,
                stopwatch.ElapsedTicks / (long)iterations);

            return stopwatch.Elapsed;
        }

        public KeyValuePair<string, object> GetRenderTestCase(int index)
        {
            switch (index % 13)
            {
                case 0:
                    return new KeyValuePair<string, object>("{{Foo}}", new { Foo = "Bar" });
                case 1:
                    return new KeyValuePair<string, object>("Just Literal Data", null);
                case 2:
                    return new KeyValuePair<string, object>("Inline Lambda {{Foo}}", new { Foo = new Func<object>(() => "Bar") });
                case 3:
                    return new KeyValuePair<string, object>("Section Lambda {{#Foo}} Section Stuff {{/Foo}}", new { Foo = new Func<string, object>(str => "Bar") });
                case 4:
                    return new KeyValuePair<string, object>("Section Lambda {{#Foo}} Child Section {{#ChildSection}} Stuff! {{/ChildSection}} {{/Foo}}", new { Foo = new { ChildSection = true } });
                case 5:
                    return new KeyValuePair<string, object>("{{Foo}} New Tags {{=<% %>=}} <% Bar %> ", new { Foo = "Bar", Bar = "Foo" });
                case 6:
                    return new KeyValuePair<string, object>("{{Foo}} New Tags {{=<% %>=}} <% Bar %> <%=<| |>=%> <|Foo|>", new { Foo = "Bar", Bar = "Foo" });
                case 7:
                    return new KeyValuePair<string, object>("{{Property}}", new StronglyTypedTestClass { Property = 1 });
                case 8:
                    return new KeyValuePair<string, object>("{{StaticField}}", new StronglyTypedTestClass());
                case 9:
                    return new KeyValuePair<string, object>("{{StaticProperty}}", new StronglyTypedTestClass());
                case 10:
                    return new KeyValuePair<string, object>("{{Field}}", new StronglyTypedTestClass { Field = 1 });
                case 11:
                    return new KeyValuePair<string, object>("{{StaticMethodWithNoArgs}}", new StronglyTypedTestClass());
                case 12:
                    return new KeyValuePair<string, object>("{{^StaticMethodWithArgs}}Stuff{{/StaticMethodWithArgs}}", new StronglyTypedTestClass());
                default:
                    return new KeyValuePair<string, object>("Not Happening", null);
            }
        }
    }
}
