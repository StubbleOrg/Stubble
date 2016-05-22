using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Nustache.Core;

namespace Stubble.Core.Performance.Candidates
{
    internal class Nustache : BaseTestCandidate
    {
        public override TimeSpan RunTest(int iterations)
        {
            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < iterations; i++)
            {
                var testCase = GetRenderTestCase(i);
                Render.StringToString(testCase.Key, testCase.Value);
            }
            stopwatch.Stop();

            return stopwatch.Elapsed;
        }
    }
}
