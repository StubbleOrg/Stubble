using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Stubble.Test.Shared.Spec
{
    public static partial class Specs
    {
        public static ThreadLocal<int> globalInt = new ThreadLocal<int>(() => 0);

        public static IEnumerable<object[]> LambdaTests => new List<SpecTest>
        {
            new SpecTest
            {
                Name = "Interpolation",
                Desc = "A lambda's return value should be interpolated.",
                Data = new { lambda = new Func<object>(() => "world") },
                Template = @"Hello, {{lambda}}!",
                Expected = @"Hello, world!"
            },
            new SpecTest
            {
                Name = "Interpolation - Expansion",
                Desc = "A lambda's return value should be parsed.",
                Data = new { planet = "world", lambda = new Func<object>(() => "{{planet}}") },
                Template = @"Hello, {{lambda}}!",
                Expected = @"Hello, world!"
            },
            new SpecTest
            {
                Name = "Interpolation - Alternate Delimiters",
                Desc = "A lambda's return value should parse with the default delimiters.",
                Data = new { planet = "world", lambda = new Func<object>(() => "|planet| => {{planet}}") },
                Template = @"{{= | | =}}
Hello, (|&lambda|)!",
                Expected = @"Hello, (|planet| => world)!"
            },
            new SpecTest
            {
                Name = "Interpolation - Multiple Calls",
                Desc = "Interpolated lambdas should not be cached.",
                Data = new { lambda = new Func<object>(() => ++globalInt.Value) },
                Template = @"{{lambda}} == {{{lambda}}} == {{lambda}}",
                Expected = @"1 == 2 == 3"
            },
            new SpecTest
            {
                Name = "Escaping",
                Desc = "Lambda results should be appropriately escaped.",
                Data = new { lambda = new Func<object>(() => ">") },
                Template = @"<{{lambda}}{{{lambda}}}",
                Expected = @"<&gt;>"
            },
            new SpecTest
            {
                Name = "Section",
                Desc = "Lambdas used for sections should receive the raw section string.",
                Data = new { x = "Error!", lambda = new Func<string, object>(text => text == "{{x}}" ? "yes" : "no") },
                Template = @"<{{#lambda}}{{x}}{{/lambda}}>",
                Expected = @"<yes>"
            },
            new SpecTest
            {
                Name = "Section - Expansion",
                Desc = "Lambdas used for sections should have their results parsed.",
                Data = new { planet = "Earth", lambda = new Func<string, object>(text => text + "{{planet}}" + text) },
                Template = @"<{{#lambda}}-{{/lambda}}>",
                Expected = @"<-Earth->"
            },
            new SpecTest
            {
                Name = "Section - Alternate Delimiters",
                Desc = "Lambdas used for sections should parse with the current delimiters.",
                Data = new { planet = "Earth", lambda = new Func<string, object>(text => text + "{{planet}} => |planet|" + text) },
                Template = @"{{= | | =}}<|#lambda|-|/lambda|>",
                Expected = @"<-{{planet}} => Earth->"
            },
            new SpecTest
            {
                Name = "Section - Multiple Calls",
                Desc = "Lambdas used for sections should not be cached.",
                Data = new { lambda = new Func<string, object>(txt => "__" + txt + "__") },
                Template = @"{{#lambda}}FILE{{/lambda}} != {{#lambda}}LINE{{/lambda}}",
                Expected = @"__FILE__ != __LINE__"
            },
            new SpecTest
            {
                Name = "Inverted Section",
                Desc = "Lambdas used for inverted sections should be considered truthy.",
                Data = new { stat = "static", lambda = new Func<string, object>(text => false) },
                Template = @"<{{^lambda}}{{stat}}{{/lambda}}>",
                Expected = @"<>"
            }
        }.Select(s => new object[] { s });
    }
}
