using System.Collections.Generic;
using System.Linq;

namespace Stubble.Test.Shared.Spec
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

        public static IEnumerable<object[]> SpecTestsWithLambda
        {
            get
            {
                globalInt.Value = 0;
                return SpecTests.Concat(LambdaTests);
            }
        }
    };
}
