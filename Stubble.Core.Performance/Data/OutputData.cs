using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Stubble.Core.Performance.Data
{
    public struct OutputData
    {
        public readonly string Name;
        public readonly Dictionary<int, List<TimeSpan>> IncrementResults;
        [JsonIgnore]
        public readonly ConsoleColor OutputColor;

        public Dictionary<int, double> IncrementResultsAverage
        {
            get { return IncrementResults.ToDictionary(k => k.Key, x => x.Value.Average(y => y.TotalMilliseconds)); }
        }
        public Dictionary<int, double> RelativeValues
        {
            get
            {
                return IncrementResultsAverage.ToDictionary(
                    y => y.Key,
                    x =>
                        (x.Value /
                         Program.Outputs.Min(z => z.IncrementResultsAverage[x.Key])  * 100)
                    );
            }
        }

        [JsonIgnore]
        public readonly Func<int, TimeSpan> Test;

        public OutputData(string name, Func<int, TimeSpan> test, ConsoleColor outputColor)
        {
            Name = name;
            Test = test;
            OutputColor = outputColor;
            IncrementResults = new Dictionary<int, List<TimeSpan>>();
            foreach (var increment in Program.Increments)
            {
                IncrementResults.Add(increment, new List<TimeSpan>(Program.Iterations));
            }
        }

        public void AddIncrement(int increment, TimeSpan result)
        {
            IncrementResults[increment].Add(result);
        }
    }
}