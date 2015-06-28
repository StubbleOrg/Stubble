using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Xunit.Abstractions;

namespace Stubble.Core.Performance
{
    public class Program
    {
        internal class NothingWriter : ITestOutputHelper
        {
            public void WriteLine(string format, params object[] args)
            {
            }

            public void WriteLine(string message)
            {
            }
        }

        public static Dictionary<string, Dictionary<int, List<TimeSpan>>> Durations = new Dictionary<string, Dictionary<int, List<TimeSpan>>>();

        public static Dictionary<int, Dictionary<string, List<TimeSpan>>> DurationByIncrement = new Dictionary<int, Dictionary<string, List<TimeSpan>>>();

        readonly static PerformanceTest PerformanceTest = new PerformanceTest(new NothingWriter());
        readonly static Dictionary<string, Func<int, TimeSpan>> TestFunctions = new Dictionary<string, Func<int, TimeSpan>>
        {
            { "Stubble (Without Cache)", PerformanceTest.Simple_Template_Test },
            { "Stubble (With Cache)", PerformanceTest.Simple_Template_Test_With_Cache },
            { "Nustache", PerformanceTest.Simple_Template_Test_Nustache }
        };

        public static void Main(string[] args)
        {

            const int iterations = 10;

            var increments = new[] { 100, 1000, 10000, 100000, 1000000 };
            
            for (var i = 1; i <= iterations; i++)
            {
                Console.WriteLine("Iteration {0}", i);

                foreach (var increment in increments)
                {
                    if (!DurationByIncrement.ContainsKey(increment))
                        DurationByIncrement.Add(increment, new Dictionary<string, List<TimeSpan>>());
                    var item = DurationByIncrement[increment];

                    foreach (var testFunction in TestFunctions)
                    {
                        Console.WriteLine("****** {0} ******", testFunction.Key.ToUpper());
                        if (!item.ContainsKey(testFunction.Key))
                            item.Add(testFunction.Key, new List<TimeSpan>());

                        var timeElapsed = testFunction.Value(increment);
                        item[testFunction.Key].Add(timeElapsed);
                        Console.WriteLine("iteration {0:N0}: {1} ({2})", increment, timeElapsed.Humanize(), timeElapsed);
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Done");
            WriteOutputCsv();
            Console.ReadLine();
        }

        public static void WriteOutputCsv()
        {
            using (var writer = new StreamWriter("./results.csv"))
            {
                writer.WriteLine(string.Join(",", "Increment", string.Join(",", TestFunctions.Keys)));
                foreach (var item in DurationByIncrement)
                {
                    var res = item.Value.Select(x => x.Value.Average(y => y.TotalMilliseconds).ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine(string.Join(",", item.Key.ToString(), string.Join(",", res)));
                }
            }
        }
    }
}
