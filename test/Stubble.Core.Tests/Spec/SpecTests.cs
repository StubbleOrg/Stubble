using System;
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
            var stubble = new StubbleRenderer();
            var output = data.Partials != null ? stubble.Render(data.Template, data.Data, data.Partials) : stubble.Render(data.Template, data.Data);

            OutputStream.WriteLine("Expected \"{0}\", Actual \"{1}\"", data.Expected, output);
            var dic = stubble.Writer.Cache as IDictionary;
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

    [Collection("LambdaTestsCollection")]
    public class LambdaTests : SpecTestBase
    {
        public static int GlobalInt;

        [Theory, MemberData("Spec_Lambdas")]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }

        public static IEnumerable<object[]> Spec_Lambdas()
        {
            return new[]
            {
                new SpecTest()
                {
                    Name = "Interpolation",
                    Desc = "A lambda's return value should be interpolated.",
                    Data = new Dictionary<string, object>
                    {
                        { "lambda", new Func<object>(() => "world")}
                    },
                    Template = "Hello, {{lambda}}!",
                    Expected = "Hello, world!"
                },
                new SpecTest()
                {
                    Name = "Interpolation - Expansion",
                    Desc = "A lambda's return value should be parsed.",
                    Data = new Dictionary<string, object>
                    {
                        { "planet", "world"},
                        { "lambda", new Func<object>(() => "{{planet}}")}
                    },
                    Template = "Hello, {{lambda}}!",
                    Expected = "Hello, world!"
                },
                new SpecTest()
                {
                    Name = "Interpolation - Alternate Delimiters",
                    Desc = "A lambda's return value should parse with the default delimiters.",
                    Data = new Dictionary<string, object>
                    {
                        { "planet", "world"},
                        { "lambda", new Func<object>(() => "|planet| => {{planet}}")}
                    },
                    Template = "{{= | | =}}\nHello, (|&lambda|)!",
                    Expected = "Hello, (|planet| => world)!"
                },
                new SpecTest()
                {
                    Name = "Interpolation - Multiple Calls",
                    Desc = "Interpolated lambdas should not be cached.",
                    Data = new Dictionary<string, object>
                    {
                        { "lambda", new Func<object>(() => ++LambdaTests.GlobalInt)}
                    },
                    Template = "{{lambda}} == {{lambda}} == {{lambda}}",
                    Expected = "1 == 2 == 3"
                },
                new SpecTest()
                {
                    Name = "Escaping",
                    Desc = "Lambda results should be appropriately escaped.",
                    Data = new Dictionary<string, object>
                    {
                        { "lambda", new Func<object>(() => ">") }
                    },
                    Template = "<{{lambda}}{{{lambda}}}",
                    Expected = "<&gt;>"
                },
                new SpecTest()
                {
                    Name = "Section",
                    Desc = "Lambdas used for sections should receive the raw section string.",
                    Data = new Dictionary<string, object>
                    {
                        { "x", "error"},
                        { "lambda", new Func<string, object>(txt => txt == "{{x}}" ? "yes" : "no") }
                    },
                    Template = "<{{#lambda}}{{x}}{{/lambda}}>",
                    Expected = "<yes>"
                },
                new SpecTest()
                {
                    Name = "Section - Expansion",
                    Desc = "Lambdas used for sections should have their results parsed.",
                    Data = new Dictionary<string, object>
                    {
                        { "planet", "Earth"},
                        { "lambda", new Func<string, object>(txt => txt + "{{planet}}" + txt ) }
                    },
                    Template = "<{{#lambda}}-{{/lambda}}>",
                    Expected = "<-Earth->"
                },
                new SpecTest()
                {
                    Name = "Section - Alternate Delimiters",
                    Desc = "Lambdas used for sections should parse with the current delimiters.",
                    Data = new Dictionary<string, object>
                    {
                        { "planet", "Earth"},
                        { "lambda", new Func<string, object>(txt => txt + "{{planet}} => |planet|" + txt ) }
                    },
                    Template = "{{= | | =}}<|#lambda|-|/lambda|>",
                    Expected = "<-{{planet}} => Earth->"
                },
                new SpecTest()
                {
                    Name = "Section - Multiple Calls",
                    Desc = "Lambdas used for sections should not be cached.",
                    Data = new Dictionary<string, object>
                    {
                        { "lambda", new Func<string, object>(txt => "__" + txt + "__" ) }
                    },
                    Template = "{{#lambda}}FILE{{/lambda}} != {{#lambda}}LINE{{/lambda}}",
                    Expected = "__FILE__ != __LINE__"
                },
                new SpecTest()
                {
                    Name = "Inverted Section",
                    Desc = "Lambdas used for inverted sections should be considered truthy.",
                    Data = new Dictionary<string, object>
                    {
                        { "static", "static"},
                        { "lambda", new Func<string, object>(txt => false) }
                    },
                    Template = "<{{^lambda}}{{static}}{{/lambda}}>",
                    Expected = "<>"
                }
            }.Select(x => new [] { x });
        }

        public LambdaTests(ITestOutputHelper output)
            : base(output)
        {
        }
    }
}
