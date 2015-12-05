using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Stubble.Core.Performance.Candidates
{
    internal class StubbleCache : BaseTestCandidate
    {
        public StubbleCache(ITestOutputHelper output)
            : base(output)
        {
        }

        public override TimeSpan RunTest(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            var stubble = new StubbleRenderer();

            for (var i = 0; i < iterations; i++)
            {
                var testCase = GetRenderTestCase(i);
                stubble.Render(testCase.Key, testCase.Value);
            }
            stopwatch.Stop();

            OutputStream.WriteLine("Time Taken: {0} Milliseconds for {1:N0} iterations\nAverage {2} Ticks",
                stopwatch.ElapsedMilliseconds,
                iterations,
                stopwatch.ElapsedTicks / (long)iterations);

            return stopwatch.Elapsed;
        }
    }
}
