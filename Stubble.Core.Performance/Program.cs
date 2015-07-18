using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Humanizer;
using Newtonsoft.Json;
using Stubble.Core.Performance.Data;
using Xunit.Abstractions;

namespace Stubble.Core.Performance
{
    public class Program
    {
        public const int Iterations = 10;
        public static readonly int[] Increments = {100, 1000, 10000, 100000, 1000000 };
        public static bool ShouldLog { get; set; }
        public static bool ShouldHaltOnEnd { get; set; }

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
            new OutputData("Stubble (Without Cache)", PerformanceTest.Simple_Template_Test, ConsoleColor.White),
            new OutputData("Stubble (With Cache)", PerformanceTest.Simple_Template_Test_With_Cache, ConsoleColor.Yellow),
            new OutputData("Nustache", PerformanceTest.Simple_Template_Test_Nustache, ConsoleColor.DarkGray),
        };

        public static void Main(string[] args)
        {
            ShouldHaltOnEnd = args.Length < 1 || bool.Parse(args[0]);
            ShouldLog = args.Length < 2 || bool.Parse(args[1]);

            for (var i = 1; i <= Iterations; i++)
            {
                ConsoleExtensions.WriteLineGreen("Iteration {0}", i);

                foreach (var increment in Increments)
                {
                    RunIncrement(increment);
                    if(ShouldLog) Console.WriteLine();
                }
            }
            WriteOutputs(DateTime.UtcNow);
            if (ShouldHaltOnEnd) Console.ReadLine();
        }

        public static void RunIncrement(int increment)
        {
            foreach (var output in Outputs)
            {
                if (ShouldLog) Console.WriteLine("****** {0} ******", output.Name.ToUpper());
                var timeElapsed = output.Test(increment);
                output.AddIncrement(increment, timeElapsed);
                if (ShouldLog) ConsoleExtensions.WriteLineColor(output.OutputColor, "iteration {0:N0}: {1} ({2})", increment, timeElapsed.Humanize(), timeElapsed);
            }
        }

        public static void WriteOutputs(DateTime now)
        {
            var outputDir = string.Format("./Perf/{0:dd-MM-yyyy}", now);
            CreateDirectoryIfNotExists(outputDir);
            WriteJson(outputDir, now);
            WriteOutputCsv(outputDir, now);
        }

        public static void WriteJson(string dir, DateTime now)
        {
            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            using (var sw = new StreamWriter(string.Format("{0}/results-{1:H-mm-ss}.json", dir, now)))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, Outputs);
            }
        }

        public static void WriteOutputCsv(string dir, DateTime now)
        {
            using (var writer = new StreamWriter(string.Format("{0}/results-{1:H-mm-ss}.csv", dir, now)))
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

        public static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
