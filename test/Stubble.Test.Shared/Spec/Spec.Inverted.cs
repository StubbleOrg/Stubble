using System;
using System.Collections.Generic;
using System.Linq;

namespace Stubble.Test.Shared.Spec
{
    public static partial class Specs
    {
        public static IEnumerable<object[]> InvertedTests => new List<SpecTest>
        {
            new SpecTest {
                Name = @"Falsey",
                Desc = @"Falsey sections should have their contents rendered.",
                Data = new { boolean = false, },
                Template = @"""{{^boolean}}This should be rendered.{{/boolean}}""",
                Expected = @"""This should be rendered.""",
                Partials = null
            },
            new SpecTest {
                Name = @"Truthy",
                Desc = @"Truthy sections should have their contents omitted.",
                Data = new { boolean = true, },
                Template = @"""{{^boolean}}This should not be rendered.{{/boolean}}""",
                Expected = @"""""",
                Partials = null
            },
            new SpecTest {
                Name = @"Context",
                Desc = @"Objects and hashes should behave like truthy values.",
                Data = new { context = new { name = @"Joe", }, },
                Template = @"""{{^context}}Hi {{name}}.{{/context}}""",
                Expected = @"""""",
                Partials = null
            },
            new SpecTest {
                Name = @"List",
                Desc = @"Lists should behave like truthy values.",
                Data = new { list = new [] { new { n = 1, }, new { n = 2, }, new { n = 3, } }, },
                Template = @"""{{^list}}{{n}}{{/list}}""",
                Expected = @"""""",
                Partials = null
            },
            new SpecTest {
                Name = @"Empty List",
                Desc = @"Empty lists should behave like falsey values.",
                Data = new { list = Array.Empty<object>(), },
                Template = @"""{{^list}}Yay lists!{{/list}}""",
                Expected = @"""Yay lists!""",
                Partials = null
            },
            new SpecTest {
                Name = @"Doubled",
                Desc = @"Multiple inverted sections per template should be permitted.",
                Data = new { boolean = false,  two = @"second", },
                Template = @"{{^boolean}}
* first
{{/boolean}}
* {{two}}
{{^boolean}}
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
                Name = @"Nested (Falsey)",
                Desc = @"Nested falsey sections should have their contents rendered.",
                Data = new { boolean = false, },
                Template = @"| A {{^boolean}}B {{^boolean}}C{{/boolean}} D{{/boolean}} E |",
                Expected = @"| A B C D E |",
                Partials = null
            },
            new SpecTest {
                Name = @"Nested (Truthy)",
                Desc = @"Nested truthy sections should be omitted.",
                Data = new { boolean = true, },
                Template = @"| A {{^boolean}}B {{^boolean}}C{{/boolean}} D{{/boolean}} E |",
                Expected = @"| A  E |",
                Partials = null
            },
            new SpecTest {
                Name = @"Context Misses",
                Desc = @"Failed context lookups should be considered falsey.",
                Data = new { },
                Template = @"[{{^missing}}Cannot find key 'missing'!{{/missing}}]",
                Expected = @"[Cannot find key 'missing'!]",
                Partials = null
            },
            new SpecTest {
                Name = @"Dotted Names - Truthy",
                Desc = @"Dotted names should be valid for Inverted Section tags.",
                Data = new { a = new { b = new { c = true, }, }, },
                Template = @"""{{^a.b.c}}Not Here{{/a.b.c}}"" == """"",
                Expected = @""""" == """"",
                Partials = null
            },
            new SpecTest {
                Name = @"Dotted Names - Falsey",
                Desc = @"Dotted names should be valid for Inverted Section tags.",
                Data = new { a = new { b = new { c = false, }, }, },
                Template = @"""{{^a.b.c}}Not Here{{/a.b.c}}"" == ""Not Here""",
                Expected = @"""Not Here"" == ""Not Here""",
                Partials = null
            },
            new SpecTest {
                Name = @"Dotted Names - Broken Chains",
                Desc = @"Dotted names that cannot be resolved should be considered falsey.",
                Data = new { a = new { }, },
                Template = @"""{{^a.b.c}}Not Here{{/a.b.c}}"" == ""Not Here""",
                Expected = @"""Not Here"" == ""Not Here""",
                Partials = null
            },
            new SpecTest {
                Name = @"Surrounding Whitespace",
                Desc = @"Inverted sections should not alter surrounding whitespace.",
                Data = new { boolean = false, },
                Template = @" | {{^boolean}}	|	{{/boolean}} | 
",
                Expected = @" | 	|	 | 
",
                Partials = null
            },
            new SpecTest {
                Name = @"Internal Whitespace",
                Desc = @"Inverted should not alter internal whitespace.",
                Data = new { boolean = false, },
                Template = @" | {{^boolean}} {{! Important Whitespace }}
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
                Data = new { boolean = false, },
                Template = @" {{^boolean}}NO{{/boolean}}
 {{^boolean}}WAY{{/boolean}}
",
                Expected = @" NO
 WAY
",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Lines",
                Desc = @"Standalone lines should be removed from the template.",
                Data = new { boolean = false, },
                Template = @"| This Is
{{^boolean}}
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
                Name = @"Standalone Indented Lines",
                Desc = @"Standalone indented lines should be removed from the template.",
                Data = new { boolean = false, },
                Template = @"| This Is
  {{^boolean}}
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
                Data = new { boolean = false, },
                Template = @"|
{{^boolean}}
{{/boolean}}
|",
                Expected = @"|
|",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Without Previous Line",
                Desc = @"Standalone tags should not require a newline to precede them.",
                Data = new { boolean = false, },
                Template = @"  {{^boolean}}
^{{/boolean}}
/",
                Expected = @"^
/",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Without Newline",
                Desc = @"Standalone tags should not require a newline to follow them.",
                Data = new { boolean = false, },
                Template = @"^{{^boolean}}
/
  {{/boolean}}",
                Expected = @"^
/
",
                Partials = null
            },
            new SpecTest {
                Name = @"Padding",
                Desc = @"Superfluous in-tag whitespace should be ignored.",
                Data = new { boolean = false, },
                Template = @"|{{^ boolean }}={{/ boolean }}|",
                Expected = @"|=|",
                Partials = null
            },
        }.Select(s => new object[] { s });
    }
}
