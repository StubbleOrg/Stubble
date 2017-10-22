using System.Collections.Generic;
using System.Linq;

namespace Stubble.Core.Tests.Data
{
    public static partial class Specs
    {
        public static IEnumerable<object[]> SpecTests
            => Enumerable.Empty<object[]>()
                .Concat(CommentTests)
                .Concat(DelimitersTests)
                .Concat(InterpolationTests)
                .Concat(InvertedTests)
                .Concat(PartialTests)
                .Concat(SectionTests);
    };
}
