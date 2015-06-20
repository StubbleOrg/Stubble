using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Stubble.Core.Tests.Spec
{
    public class SpecTestBase
    {
        internal readonly ITestOutputHelper OutputStream;

        public SpecTestBase(ITestOutputHelper output)
        {
            OutputStream = output;
        }

        public void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            var stubble = new Stubble();
            var output = data.Partials != null ? stubble.Render(data.Template, data.Data, data.Partials) : stubble.Render(data.Template, data.Data);

            OutputStream.WriteLine("Expected \"{0}\", Actual \"{1}\"", data.Expected, output);
            var dic = stubble.Writer.Cache as IDictionary;
            OutputStream.WriteLine("Input Data: {0}", JsonConvert.SerializeObject(data));
            OutputStream.WriteLine("Cache Data: {0}", JsonConvert.SerializeObject(dic));
            Assert.Equal(data.Expected, output);
        }
    }

    [Collection("SpecCommentTests")]
    public class CommentsTests : SpecTestBase
    {
        [Theory, MemberData("Spec_Comments")]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }

        public static IEnumerable<object[]> Spec_Comments()
        {
            return SpecTestHelper.GetTests("comments").Select(test => new object[] { test });
        }

        public CommentsTests(ITestOutputHelper output) : base(output)
        {
        }
    }

    [Collection("SpecDelimiterTests")]
    public class DelimiterTests : SpecTestBase
    {
        [Theory, MemberData("Spec_Delimiters")]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }

        public static IEnumerable<object[]> Spec_Delimiters()
        {
            return SpecTestHelper.GetTests("delimiters").Select(test => new object[] { test });
        }

        public DelimiterTests(ITestOutputHelper output) : base(output)
        {
        }
    }

    [Collection("SpecInterpolationTests")]
    public class InterpolationTests : SpecTestBase
    {
        [Theory, MemberData("Spec_Interpolation")]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }

        public static IEnumerable<object[]> Spec_Interpolation()
        {
            return SpecTestHelper.GetTests("interpolation").Select(test => new object[] { test });
        }

        public InterpolationTests(ITestOutputHelper output) : base(output)
        {
        }
    }

    [Collection("InvertedTestsCollection")]
    public class InvertedTests : SpecTestBase
    {
        [Theory, MemberData("Spec_Inverted")]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }

        public static IEnumerable<object[]> Spec_Inverted()
        {
            return SpecTestHelper.GetTests("inverted").Select(test => new object[] { test });
        }

        public InvertedTests(ITestOutputHelper output) : base(output)
        {
        }
    }

    [Collection("PartialsTestsCollection")]
    public class PartialsTests : SpecTestBase
    {
        [Theory, MemberData("Spec_Partials")]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }

        public static IEnumerable<object[]> Spec_Partials()
        {
            return SpecTestHelper.GetTests("partials").Select(test => new object[] { test });
        }

        public PartialsTests(ITestOutputHelper output) : base(output)
        {
        }
    }

    [Collection("SectionsTestsCollection")]
    public class SectionsTests : SpecTestBase
    {
        [Theory, MemberData("Spec_Sections")]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }

        public static IEnumerable<object[]> Spec_Sections()
        {
            return SpecTestHelper.GetTests("sections").Select(test => new object[] { test });
        }

        public SectionsTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}
