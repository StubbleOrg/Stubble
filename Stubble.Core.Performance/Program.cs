using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;
using Humanizer;
using Newtonsoft.Json;
using Stubble.Core.Performance.Data;
using Xunit.Abstractions;

namespace Stubble.Core.Performance
{
    public class Program
    {
        public static int Iterations = 10;
        public static readonly int[] Increments = {100, 1000, 10000, 100000, 1000000 };
        public static ProgramOptions Options;
        public static Stopwatch GlobalStopwatch;

        private static readonly ITestOutputHelper Writer = new NothingWriter();

        public static readonly List<OutputData> Outputs = new List<OutputData>
        {
            new OutputData("Stubble (Without Cache)", new Candidates.StubbleNoCache(Writer), ConsoleColor.Yellow),
            new OutputData("Stubble (With Cache)", new Candidates.StubbleCache(Writer), ConsoleColor.DarkYellow),
            new OutputData("Nustache", new Candidates.Nustache(Writer), ConsoleColor.Cyan)
        };

        public static void Main(string[] args)
        {
            Options = new ProgramOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, Options)) return;

            GlobalStopwatch = Stopwatch.StartNew();
            if (!Options.ShowTitles && Options.ShouldLog)
            {
                foreach (var output in Outputs)
                {
                    ConsoleExtensions.WriteLineColor(output.OutputColor, output.Name);
                }
            }

            Iterations = Options.NumberOfIterations;
            for (var i = 1; i <= Iterations; i++)
            {
                ConsoleExtensions.WriteLine(string.Format("Iteration {0}", i).ToUpper());

                foreach (var increment in Increments)
                {
                    RunIncrement(increment);
                }
            }
            GlobalStopwatch.Stop();
            if (Options.ShouldOutput) WriteOutputs(DateTime.UtcNow);
            if (Options.ShouldHaltOnEnd) ConsoleExtensions.WriteLine("DONE");
            if (Options.ShouldHaltOnEnd) Console.ReadLine();
        }

        public static void RunIncrement(int increment)
        {
            foreach (var output in Outputs)
            {
                if (Options.ShouldLog && Options.ShowTitles) ConsoleExtensions.WriteLineColor(output.OutputColor, "****** {0} ******", output.Name.ToUpper());
                var timeElapsed = output.Candidate.RunTest(increment);
                output.AddIncrement(increment, timeElapsed);
                if (Options.ShouldLog) ConsoleExtensions.WriteLineColor(output.OutputColor, "Iteration {0:N0}\t: {1} ({2})", increment, timeElapsed.Humanize(), timeElapsed);
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

    public class ProgramOptions
    {
        [Option('s', "ShouldLog", DefaultValue = false, HelpText = "Should Log Output?")]
        public bool ShouldLog { get; set; }

        [Option('o', "ShouldOutput", DefaultValue = false, HelpText = "Should Output results?")]
        public bool ShouldOutput { get; set; }

        [Option('h', "ShouldHaltOnEnd", DefaultValue = false, HelpText = "Should Halt on End of Run?")]
        public bool ShouldHaltOnEnd { get; set; }

        [Option('t', "ShowTitles", DefaultValue = false, HelpText = "Should show titles?")]
        public bool ShowTitles { get; set; }

        [Option('i', "Iterations", DefaultValue = 10, HelpText = "Number of Iterations that should be run?")]
        public int NumberOfIterations { get; set; }
    }

    internal class NothingWriter : ITestOutputHelper
    {
        public void WriteLine(string format, params object[] args)
        {
        }

        public void WriteLine(string message)
        {
        }
    }
}
