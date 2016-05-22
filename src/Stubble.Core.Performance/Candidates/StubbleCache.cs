using System;
using System.Diagnostics;

namespace Stubble.Core.Performance.Candidates
{
    internal class StubbleCache : BaseTestCandidate
    {
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

            return stopwatch.Elapsed;
        }
    }
}
