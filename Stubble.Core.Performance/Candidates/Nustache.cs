using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Nustache.Compilation;
using Nustache.Core;
using Xunit.Abstractions;

namespace Stubble.Core.Performance.Candidates
{
    internal class Nustache : BaseTestCandidate
    {
        internal Dictionary<string, Expression> TemplateCache = new Dictionary<string, Expression>(20);

        public Nustache(ITestOutputHelper output)
            : base(output)
        {
        }

        public override TimeSpan RunTest(int iterations)
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
    }
}
