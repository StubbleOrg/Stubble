using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Newtonsoft.Json;
using Stubble.Core.Performance.Data;
using Xunit.Abstractions;

namespace Stubble.Core.Performance
{
    public class Program
    {
        public const int Iterations = 10;
        public static readonly int[] Increments = { 100 , 1000, 10000, 100000, 1000000 };

        internal class NothingWriter : ITestOutputHelper
        {
            public void WriteLine(string format, params object[] args)
            {
            }

            public void WriteLine(string message)
            {
            }
        }

        readonly static PerformanceTest PerformanceTest = new PerformanceTest(new NothingWriter());

        public static readonly List<OutputData> Outputs = new List<OutputData>
        {
            new OutputData("Stubble (Without Cache)", PerformanceTest.Simple_Template_Test),
            new OutputData("Stubble (With Cache)", PerformanceTest.Simple_Template_Test_With_Cache),
            new OutputData("Nustache", PerformanceTest.Simple_Template_Test_Nustache),
        };

        public static void Main(string[] args)
        {
            for (var i = 1; i <= Iterations; i++)
            {
                ConsoleExtensions.WriteLineGreen("Iteration {0}", i);

                foreach (var increment in Increments)
                {
                    RunIncrement(increment);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Done");
            WriteJson();
            WriteOutputCsv();
            Console.ReadLine();
        }

        public static void RunIncrement(int increment)
        {
            foreach (var output in Outputs)
            {
                Console.WriteLine("****** {0} ******", output.Name.ToUpper());
                var timeElapsed = output.Test(increment);
                output.AddIncrement(increment, timeElapsed);
                Console.WriteLine("iteration {0:N0}: {1} ({2})", increment, timeElapsed.Humanize(), timeElapsed);
            }
        }

        public static void WriteJson()
        {
            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            using (var sw = new StreamWriter(string.Format("./results-{0:dd-MM-yyyy}.json", DateTime.UtcNow)))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, Outputs);
            }
        }

        public static void WriteOutputCsv()
        {
            using (var writer = new StreamWriter("./results.csv"))
            {
                writer.WriteLine(string.Join(",", "Increment", string.Join(",", Outputs.Select(x => x.Name))));
                foreach (var increment in Increments)
                {
                    var incrementVal = increment;
                    writer.WriteLine(string.Join(",", incrementVal, string.Join(",", Outputs.Select(x => x.IncrementResultsAverage[incrementVal].ToString(CultureInfo.InvariantCulture)))));
                }
                writer.WriteLine(new string(',', Outputs.Count + 1));
                foreach (var increment in Increments)
                {
                    var incrementVal = increment;
                    writer.WriteLine(string.Join(",", incrementVal, string.Join(",", Outputs.Select(x => x.RelativeValues[incrementVal].ToString(CultureInfo.InvariantCulture)))));
                }
            }
        }
    }

    public static class ConsoleExtensions
    {
        public static void WriteLineGreen(string line, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(line, args);
            Console.ResetColor();
        }
    }
}
