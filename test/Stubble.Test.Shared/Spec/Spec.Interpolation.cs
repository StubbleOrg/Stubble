using System.Collections.Generic;
using System.Linq;

namespace Stubble.Test.Shared.Spec
{
    public static partial class Specs
    {
        public static IEnumerable<object[]> InterpolationTests => new List<SpecTest>
        {
            new SpecTest {
                Name = @"No Interpolation",
                Desc = @"Mustache-free templates should render as-is.",
                Data = new { },
                Template = @"Hello from {Mustache}!
",
                Expected = @"Hello from {Mustache}!
"
            },
            new SpecTest {
                Name = @"Basic Interpolation",
                Desc = @"Unadorned tags should interpolate content into the template.",
                Data = new { subject = @"world", },
                Template = @"Hello, {{subject}}!
",
                Expected = @"Hello, world!
"
            },
            new SpecTest {
                Name = @"HTML Escaping",
                Desc = @"Basic interpolation should be HTML escaped.",
                Data = new { forbidden = @"& "" < >", },
                Template = @"These characters should be HTML escaped: {{forbidden}}
",
                Expected = @"These characters should be HTML escaped: &amp; &quot; &lt; &gt;
"
            },
            new SpecTest {
                Name = @"Triple Mustache",
                Desc = @"Triple mustaches should interpolate without HTML escaping.",
                Data = new { forbidden = @"& "" < >", },
                Template = @"These characters should not be HTML escaped: {{{forbidden}}}
",
                Expected = @"These characters should not be HTML escaped: & "" < >
"
            },
            new SpecTest {
                Name = @"Ampersand",
                Desc = @"Ampersand should interpolate without HTML escaping.",
                Data = new { forbidden = @"& "" < >", },
                Template = @"These characters should not be HTML escaped: {{&forbidden}}
",
                Expected = @"These characters should not be HTML escaped: & "" < >
"
            },
            new SpecTest {
                Name = @"Basic Integer Interpolation",
                Desc = @"Integers should interpolate seamlessly.",
                Data = new { mph = 85, },
                Template = @"""{{mph}} miles an hour!""",
                Expected = @"""85 miles an hour!"""
            },
            new SpecTest {
                Name = @"Triple Mustache Integer Interpolation",
                Desc = @"Integers should interpolate seamlessly.",
                Data = new { mph = 85, },
                Template = @"""{{{mph}}} miles an hour!""",
                Expected = @"""85 miles an hour!"""
            },
            new SpecTest {
                Name = @"Ampersand Integer Interpolation",
                Desc = @"Integers should interpolate seamlessly.",
                Data = new { mph = 85, },
                Template = @"""{{&mph}} miles an hour!""",
                Expected = @"""85 miles an hour!"""
            },
            new SpecTest {
                Name = @"Basic Decimal Interpolation",
                Desc = @"Decimals should interpolate seamlessly with proper significance.",
                Data = new { power = 1.21, },
                Template = @"""{{power}} jiggawatts!""",
                Expected = @"""1.21 jiggawatts!"""
            },
            new SpecTest {
                Name = @"Triple Mustache Decimal Interpolation",
                Desc = @"Decimals should interpolate seamlessly with proper significance.",
                Data = new { power = 1.21, },
                Template = @"""{{{power}}} jiggawatts!""",
                Expected = @"""1.21 jiggawatts!"""
            },
            new SpecTest {
                Name = @"Ampersand Decimal Interpolation",
                Desc = @"Decimals should interpolate seamlessly with proper significance.",
                Data = new { power = 1.21, },
                Template = @"""{{&power}} jiggawatts!""",
                Expected = @"""1.21 jiggawatts!"""
            },
            new SpecTest {
                Name = @"Basic Context Miss Interpolation",
                Desc = @"Failed context lookups should default to empty strings.",
                Data = new { },
                Template = @"I ({{cannot}}) be seen!",
                Expected = @"I () be seen!"
            },
            new SpecTest {
                Name = @"Triple Mustache Context Miss Interpolation",
                Desc = @"Failed context lookups should default to empty strings.",
                Data = new { },
                Template = @"I ({{{cannot}}}) be seen!",
                Expected = @"I () be seen!"
            },
            new SpecTest {
                Name = @"Ampersand Context Miss Interpolation",
                Desc = @"Failed context lookups should default to empty strings.",
                Data = new { },
                Template = @"I ({{&cannot}}) be seen!",
                Expected = @"I () be seen!"
            },
            new SpecTest {
                Name = @"Dotted Names - Basic Interpolation",
                Desc = @"Dotted names should be considered a form of shorthand for sections.",
                Data = new { person = new { name = @"Joe", }, },
                Template = @"""{{person.name}}"" == ""{{#person}}{{name}}{{/person}}""",
                Expected = @"""Joe"" == ""Joe"""
            },
            new SpecTest {
                Name = @"Dotted Names - Triple Mustache Interpolation",
                Desc = @"Dotted names should be considered a form of shorthand for sections.",
                Data = new { person = new { name = @"Joe", }, },
                Template = @"""{{{person.name}}}"" == ""{{#person}}{{{name}}}{{/person}}""",
                Expected = @"""Joe"" == ""Joe"""
            },
            new SpecTest {
                Name = @"Dotted Names - Ampersand Interpolation",
                Desc = @"Dotted names should be considered a form of shorthand for sections.",
                Data = new { person = new { name = @"Joe", }, },
                Template = @"""{{&person.name}}"" == ""{{#person}}{{&name}}{{/person}}""",
                Expected = @"""Joe"" == ""Joe"""
            },
            new SpecTest {
                Name = @"Dotted Names - Arbitrary Depth",
                Desc = @"Dotted names should be functional to any level of nesting.",
                Data = new { a = new { b = new { c = new { d = new { e = new { name = @"Phil", }, }, }, }, }, },
                Template = @"""{{a.b.c.d.e.name}}"" == ""Phil""",
                Expected = @"""Phil"" == ""Phil"""
            },
            new SpecTest {
                Name = @"Dotted Names - Broken Chains",
                Desc = @"Any falsey value prior to the last part of the name should yield ''.",
                Data = new { a = new { }, },
                Template = @"""{{a.b.c}}"" == """"",
                Expected = @""""" == """""
            },
            new SpecTest {
                Name = @"Dotted Names - Broken Chain Resolution",
                Desc = @"Each part of a dotted name should resolve only against its parent.",
                Data = new { a = new { b = new { }, },  c = new { name = @"Jim", }, },
                Template = @"""{{a.b.c.name}}"" == """"",
                Expected = @""""" == """""
            },
            new SpecTest {
                Name = @"Dotted Names - Initial Resolution",
                Desc = @"The first part of a dotted name should resolve as any other name.",
                Data = new { a = new { b = new { c = new { d = new { e = new { name = @"Phil", }, }, }, }, },  b = new { c = new { d = new { e = new { name = @"Wrong", }, }, }, }, },
                Template = @"""{{#a}}{{b.c.d.e.name}}{{/a}}"" == ""Phil""",
                Expected = @"""Phil"" == ""Phil"""
            },
            new SpecTest {
                Name = @"Dotted Names - Context Precedence",
                Desc = @"Dotted names should be resolved against former resolutions.",
                Data = new { a = new { b = new { }, },  b = new { c = @"ERROR", }, },
                Template = @"{{#a}}{{b.c}}{{/a}}",
                Expected = @""
            },
            new SpecTest {
                Name = @"Interpolation - Surrounding Whitespace",
                Desc = @"Interpolation should not alter surrounding whitespace.",
                Data = new { text = @"---", },
                Template = @"| {{text}} |",
                Expected = @"| --- |"
            },
            new SpecTest {
                Name = @"Triple Mustache - Surrounding Whitespace",
                Desc = @"Interpolation should not alter surrounding whitespace.",
                Data = new { text = @"---", },
                Template = @"| {{{text}}} |",
                Expected = @"| --- |"
            },
            new SpecTest {
                Name = @"Ampersand - Surrounding Whitespace",
                Desc = @"Interpolation should not alter surrounding whitespace.",
                Data = new { text = @"---", },
                Template = @"| {{&text}} |",
                Expected = @"| --- |"
            },
            new SpecTest {
                Name = @"Interpolation - Standalone",
                Desc = @"Standalone interpolation should not alter surrounding whitespace.",
                Data = new { text = @"---", },
                Template = @"  {{text}}
",
                Expected = @"  ---
"
            },
            new SpecTest {
                Name = @"Triple Mustache - Standalone",
                Desc = @"Standalone interpolation should not alter surrounding whitespace.",
                Data = new { text = @"---", },
                Template = @"  {{{text}}}
",
                Expected = @"  ---
"
            },
            new SpecTest {
                Name = @"Ampersand - Standalone",
                Desc = @"Standalone interpolation should not alter surrounding whitespace.",
                Data = new { text = @"---", },
                Template = @"  {{&text}}
",
                Expected = @"  ---
"
            },
            new SpecTest {
                Name = @"Interpolation With Padding",
                Desc = @"Superfluous in-tag whitespace should be ignored.",
                Data = new { text = @"---", },
                Template = @"|{{ text }}|",
                Expected = @"|---|"
            },
            new SpecTest {
                Name = @"Triple Mustache With Padding",
                Desc = @"Superfluous in-tag whitespace should be ignored.",
                Data = new { text = @"---", },
                Template = @"|{{{ text }}}|",
                Expected = @"|---|"
            },
            new SpecTest {
                Name = @"Ampersand With Padding",
                Desc = @"Superfluous in-tag whitespace should be ignored.",
                Data = new { text = @"---", },
                Template = @"|{{& text }}|",
                Expected = @"|---|"
            },
        }.Select(s => new object[] { s });
    }
}
