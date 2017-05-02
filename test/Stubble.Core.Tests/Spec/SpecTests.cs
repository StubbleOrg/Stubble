// <copyright file="SpecTests.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Stubble.Core.Tests.Spec
{

    [Collection("SpecCommentTests")]
    public class CommentsTests : SpecTestBase
    {
        public CommentsTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> Spec_Comments(bool skip)
        {
            return SpecTestHelper.GetTests("comments", skip).Select(test => new object[] { test });
        }

        [Theory]
        [MemberData(nameof(Spec_Comments), true)]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }
    }

    [Collection("SpecDelimiterTests")]
    public class DelimiterTests : SpecTestBase
    {
        public DelimiterTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> Spec_Delimiters(bool skip)
        {
            return SpecTestHelper.GetTests("delimiters", skip).Select(test => new object[] { test });
        }

        [Theory]
        [MemberData(nameof(Spec_Delimiters), true)]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }
    }

    [Collection("SpecInterpolationTests")]
    public class InterpolationTests : SpecTestBase
    {
        public InterpolationTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> Spec_Interpolation(bool skip)
        {
            return SpecTestHelper.GetTests("interpolation", skip).Select(test => new object[] { test });
        }

        [Theory]
        [MemberData(nameof(Spec_Interpolation), true)]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }
    }

    [Collection("InvertedTestsCollection")]
    public class InvertedTests : SpecTestBase
    {
        public InvertedTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> Spec_Inverted(bool skip)
        {
            return SpecTestHelper.GetTests("inverted", skip).Select(test => new object[] { test });
        }

        [Theory]
        [MemberData(nameof(Spec_Inverted), true)]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }
    }

    [Collection("PartialsTestsCollection")]
    public class PartialsTests : SpecTestBase
    {
        public PartialsTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> Spec_Partials(bool skip)
        {
            return SpecTestHelper.GetTests("partials", skip).Select(test => new object[] { test });
        }

        [Theory]
        [MemberData(nameof(Spec_Partials), true)]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }
    }

    [Collection("SectionsTestsCollection")]
    public class SectionsTests : SpecTestBase
    {
        public SectionsTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> Spec_Sections(bool skip)
        {
            return SpecTestHelper.GetTests("sections", skip).Select(test => new object[] { test });
        }

        [Theory]
        [MemberData(nameof(Spec_Sections), true)]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }
    }

    [Collection("LambdaTestsCollection")]
    public class LambdaTests : SpecTestBase
    {
        public static AsyncLocal<int> GlobalInt = new AsyncLocal<int>();

        public LambdaTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> Spec_Lambdas()
        {
            return new[]
            {
                new SpecTest()
                {
                    name = "Interpolation",
                    desc = "A lambda's return value should be interpolated.",
                    data = new Dictionary<string, object>
                    {
                        { "lambda", new Func<object>(() => "world") }
                    },
                    template = "Hello, {{lambda}}!",
                    expected = "Hello, world!"
                },
                new SpecTest()
                {
                    name = "Interpolation - Expansion",
                    desc = "A lambda's return value should be parsed.",
                    data = new Dictionary<string, object>
                    {
                        { "planet", "world" },
                        { "lambda", new Func<object>(() => "{{planet}}") }
                    },
                    template = "Hello, {{lambda}}!",
                    expected = "Hello, world!"
                },
                new SpecTest()
                {
                    name = "Interpolation - Alternate Delimiters",
                    desc = "A lambda's return value should parse with the default delimiters.",
                    data = new Dictionary<string, object>
                    {
                        { "planet", "world" },
                        { "lambda", new Func<object>(() => "|planet| => {{planet}}") }
                    },
                    template = "{{= | | =}}\nHello, (|&lambda|)!",
                    expected = "Hello, (|planet| => world)!"
                },
                new SpecTest()
                {
                    name = "Interpolation - Multiple Calls",
                    desc = "Interpolated lambdas should not be cached.",
                    data = new Dictionary<string, object>
                    {
                        { "lambda", new Func<object>(() => ++LambdaTests.GlobalInt.Value) }
                    },
                    template = "{{lambda}} == {{lambda}} == {{lambda}}",
                    expected = "1 == 2 == 3"
                },
                new SpecTest()
                {
                    name = "Escaping",
                    desc = "Lambda results should be appropriately escaped.",
                    data = new Dictionary<string, object>
                    {
                        { "lambda", new Func<object>(() => ">") }
                    },
                    template = "<{{lambda}}{{{lambda}}}",
                    expected = "<&gt;>"
                },
                new SpecTest()
                {
                    name = "Section",
                    desc = "Lambdas used for sections should receive the raw section string.",
                    data = new Dictionary<string, object>
                    {
                        { "x", "error" },
                        { "lambda", new Func<string, object>(txt => txt == "{{x}}" ? "yes" : "no") }
                    },
                    template = "<{{#lambda}}{{x}}{{/lambda}}>",
                    expected = "<yes>"
                },
                new SpecTest()
                {
                    name = "Section - Expansion",
                    desc = "Lambdas used for sections should have their results parsed.",
                    data = new Dictionary<string, object>
                    {
                        { "planet", "Earth" },
                        { "lambda", new Func<string, object>(txt => txt + "{{planet}}" + txt) }
                    },
                    template = "<{{#lambda}}-{{/lambda}}>",
                    expected = "<-Earth->"
                },
                new SpecTest()
                {
                    name = "Section - Alternate Delimiters",
                    desc = "Lambdas used for sections should parse with the current delimiters.",
                    data = new Dictionary<string, object>
                    {
                        { "planet", "Earth" },
                        { "lambda", new Func<string, object>(txt => txt + "{{planet}} => |planet|" + txt) }
                    },
                    template = "{{= | | =}}<|#lambda|-|/lambda|>",
                    expected = "<-{{planet}} => Earth->"
                },
                new SpecTest()
                {
                    name = "Section - Multiple Calls",
                    desc = "Lambdas used for sections should not be cached.",
                    data = new Dictionary<string, object>
                    {
                        { "lambda", new Func<string, object>(txt => "__" + txt + "__") }
                    },
                    template = "{{#lambda}}FILE{{/lambda}} != {{#lambda}}LINE{{/lambda}}",
                    expected = "__FILE__ != __LINE__"
                },
                new SpecTest()
                {
                    name = "Inverted Section",
                    desc = "Lambdas used for inverted sections should be considered truthy.",
                    data = new Dictionary<string, object>
                    {
                        { "static", "static" },
                        { "lambda", new Func<string, object>(txt => false) }
                    },
                    template = "<{{^lambda}}{{static}}{{/lambda}}>",
                    expected = "<>"
                }
            }.Select(x => new[] { x });
        }

        [Theory]
        [MemberData(nameof(Spec_Lambdas))]
        public new void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            base.It_Can_Pass_Spec_Tests(data);
        }
    }
}
