using System;
using System.Collections.Generic;
using System.Linq;

namespace Stubble.Core.Tests.Data
{
    public static partial class Specs
    {
        public static IEnumerable<object[]> SectionTests => new List<SpecTest>
        {
            new SpecTest {
                Name = @"Truthy",
                Desc = @"Truthy sections should have their contents rendered.",
                Data = new { boolean = true, },
                Template = @"""{{#boolean}}This should be rendered.{{/boolean}}""",
                Expected = @"""This should be rendered.""",
                Partials = null
            },
            new SpecTest {
                Name = @"Falsey",
                Desc = @"Falsey sections should have their contents omitted.",
                Data = new { boolean = false, },
                Template = @"""{{#boolean}}This should not be rendered.{{/boolean}}""",
                Expected = @"""""",
                Partials = null
            },
            new SpecTest {
                Name = @"Context",
                Desc = @"Objects and hashes should be pushed onto the context stack.",
                Data = new { context = new { name = @"Joe", }, },
                Template = @"""{{#context}}Hi {{name}}.{{/context}}""",
                Expected = @"""Hi Joe.""",
                Partials = null
            },
            new SpecTest {
                Name = @"Deeply Nested Contexts",
                Desc = @"All elements on the context stack should be accessible.",
                Data = new { a = new { one = 1, },  b = new { two = 2, },  c = new { three = 3, },  d = new { four = 4, },  e = new { five = 5, }, },
                Template = @"{{#a}}
{{one}}
{{#b}}
{{one}}{{two}}{{one}}
{{#c}}
{{one}}{{two}}{{three}}{{two}}{{one}}
{{#d}}
{{one}}{{two}}{{three}}{{four}}{{three}}{{two}}{{one}}
{{#e}}
{{one}}{{two}}{{three}}{{four}}{{five}}{{four}}{{three}}{{two}}{{one}}
{{/e}}
{{one}}{{two}}{{three}}{{four}}{{three}}{{two}}{{one}}
{{/d}}
{{one}}{{two}}{{three}}{{two}}{{one}}
{{/c}}
{{one}}{{two}}{{one}}
{{/b}}
{{one}}
{{/a}}
",
                Expected = @"1
121
12321
1234321
123454321
1234321
12321
121
1
",
                Partials = null
            },
            new SpecTest {
                Name = @"List",
                Desc = @"Lists should be iterated; list items should visit the context stack.",
                Data = new { list = new [] { new { item = 1, }, new { item = 2, }, new { item = 3, } }, },
                Template = @"""{{#list}}{{item}}{{/list}}""",
                Expected = @"""123""",
                Partials = null
            },
            new SpecTest {
                Name = @"Empty List",
                Desc = @"Empty lists should behave like falsey values.",
                Data = new { list = Array.Empty<object>(), },
                Template = @"""{{#list}}Yay lists!{{/list}}""",
                Expected = @"""""",
                Partials = null
            },
            new SpecTest {
                Name = @"Doubled",
                Desc = @"Multiple sections per template should be permitted.",
                Data = new { boolean = true,  two = @"second", },
                Template = @"{{#boolean}}
* first
{{/boolean}}
* {{two}}
{{#boolean}}
* third
{{/boolean}}
",
                Expected = @"* first
* second
* third
",
                Partials = null
            },
            new SpecTest {
                Name = @"Nested (Truthy)",
                Desc = @"Nested truthy sections should have their contents rendered.",
                Data = new { boolean = true, },
                Template = @"| A {{#boolean}}B {{#boolean}}C{{/boolean}} D{{/boolean}} E |",
                Expected = @"| A B C D E |",
                Partials = null
            },
            new SpecTest {
                Name = @"Nested (Falsey)",
                Desc = @"Nested falsey sections should be omitted.",
                Data = new { boolean = false, },
                Template = @"| A {{#boolean}}B {{#boolean}}C{{/boolean}} D{{/boolean}} E |",
                Expected = @"| A  E |",
                Partials = null
            },
            new SpecTest {
                Name = @"Context Misses",
                Desc = @"Failed context lookups should be considered falsey.",
                Data = new { },
                Template = @"[{{#missing}}Found key 'missing'!{{/missing}}]",
                Expected = @"[]",
                Partials = null
            },
            new SpecTest {
                Name = @"Implicit Iterator - String",
                Desc = @"Implicit iterators should directly interpolate strings.",
                Data = new { list = new [] { @"a", @"b", @"c", @"d", @"e" }, },
                Template = @"""{{#list}}({{.}}){{/list}}""",
                Expected = @"""(a)(b)(c)(d)(e)""",
                Partials = null
            },
            new SpecTest {
                Name = @"Implicit Iterator - Integer",
                Desc = @"Implicit iterators should cast integers to strings and interpolate.",
                Data = new { list = new [] { 1, 2, 3, 4, 5 }, },
                Template = @"""{{#list}}({{.}}){{/list}}""",
                Expected = @"""(1)(2)(3)(4)(5)""",
                Partials = null
            },
            new SpecTest {
                Name = @"Implicit Iterator - Decimal",
                Desc = @"Implicit iterators should cast decimals to strings and interpolate.",
                Data = new { list = new [] { 1.1, 2.2, 3.3, 4.4, 5.5 }, },
                Template = @"""{{#list}}({{.}}){{/list}}""",
                Expected = @"""(1.1)(2.2)(3.3)(4.4)(5.5)""",
                Partials = null
            },
            new SpecTest {
                Name = @"Implicit Iterator - Array",
                Desc = @"Implicit iterators should allow iterating over nested arrays.",
                Data = new { list = new object[][] { new object[] { 1, 2, 3 }, new [] { @"a", @"b", @"c" } }, },
                Template = @"""{{#list}}({{#.}}{{.}}{{/.}}){{/list}}""",
                Expected = @"""(123)(abc)""",
                Partials = null
            },
            new SpecTest {
                Name = @"Dotted Names - Truthy",
                Desc = @"Dotted names should be valid for Section tags.",
                Data = new { a = new { b = new { c = true, }, }, },
                Template = @"""{{#a.b.c}}Here{{/a.b.c}}"" == ""Here""",
                Expected = @"""Here"" == ""Here""",
                Partials = null
            },
            new SpecTest {
                Name = @"Dotted Names - Falsey",
                Desc = @"Dotted names should be valid for Section tags.",
                Data = new { a = new { b = new { c = false, }, }, },
                Template = @"""{{#a.b.c}}Here{{/a.b.c}}"" == """"",
                Expected = @""""" == """"",
                Partials = null
            },
            new SpecTest {
                Name = @"Dotted Names - Broken Chains",
                Desc = @"Dotted names that cannot be resolved should be considered falsey.",
                Data = new { a = new { }, },
                Template = @"""{{#a.b.c}}Here{{/a.b.c}}"" == """"",
                Expected = @""""" == """"",
                Partials = null
            },
            new SpecTest {
                Name = @"Surrounding Whitespace",
                Desc = @"Sections should not alter surrounding whitespace.",
                Data = new { boolean = true, },
                Template = @" | {{#boolean}}	|	{{/boolean}} | 
",
                Expected = @" | 	|	 | 
",
                Partials = null
            },
            new SpecTest {
                Name = @"Internal Whitespace",
                Desc = @"Sections should not alter internal whitespace.",
                Data = new { boolean = true, },
                Template = @" | {{#boolean}} {{! Important Whitespace }}
 {{/boolean}} | 
",
                Expected = @" |  
  | 
",
                Partials = null
            },
            new SpecTest {
                Name = @"Indented Inline Sections",
                Desc = @"Single-line sections should not alter surrounding whitespace.",
                Data = new { boolean = true, },
                Template = @" {{#boolean}}YES{{/boolean}}
 {{#boolean}}GOOD{{/boolean}}
",
                Expected = @" YES
 GOOD
",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Lines",
                Desc = @"Standalone lines should be removed from the template.",
                Data = new { boolean = true, },
                Template = @"| This Is
{{#boolean}}
|
{{/boolean}}
| A Line
",
                Expected = @"| This Is
|
| A Line
",
                Partials = null
            },
            new SpecTest {
                Name = @"Indented Standalone Lines",
                Desc = @"Indented standalone lines should be removed from the template.",
                Data = new { boolean = true, },
                Template = @"| This Is
  {{#boolean}}
|
  {{/boolean}}
| A Line
",
                Expected = @"| This Is
|
| A Line
",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Line Endings",
                Desc = @"""\r\n"" should be considered a newline for standalone tags.",
                Data = new { boolean = true, },
                Template = @"|
{{#boolean}}
{{/boolean}}
|",
                Expected = @"|
|",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Without Previous Line",
                Desc = @"Standalone tags should not require a newline to precede them.",
                Data = new { boolean = true, },
                Template = @"  {{#boolean}}
#{{/boolean}}
/",
                Expected = @"#
/",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Without Newline",
                Desc = @"Standalone tags should not require a newline to follow them.",
                Data = new { boolean = true, },
                Template = @"#{{#boolean}}
/
  {{/boolean}}",
                Expected = @"#
/
",
                Partials = null
            },
            new SpecTest {
                Name = @"Padding",
                Desc = @"Superfluous in-tag whitespace should be ignored.",
                Data = new { boolean = true, },
                Template = @"|{{# boolean }}={{/ boolean }}|",
                Expected = @"|=|",
                Partials = null
            },
        }.Select(s => new object[] { s });
    }
}
