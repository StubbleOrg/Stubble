using System;
using System.Collections.Generic;
using System.Linq;

namespace Stubble.Core.Tests.Data
{
    public static partial class Specs
    {
        public static IEnumerable<object[]> PartialTests => new List<SpecTest>
        {
            new SpecTest {
                Name = @"Basic Behavior",
                Desc = @"The greater-than operator should expand to the named partial.",
                Data = new { },
                Template = @"""{{>text}}""",
                Expected = @"""from partial""",
                Partials = new Dictionary<string, string> { { @"text", @"from partial" } }
            },
            new SpecTest {
                Name = @"Failed Lookup",
                Desc = @"The empty string should be used when the named partial is not found.",
                Data = new { },
                Template = @"""{{>text}}""",
                Expected = @"""""",
                Partials = new Dictionary<string, string> {  }
            },
            new SpecTest {
                Name = @"Context",
                Desc = @"The greater-than operator should operate within the current context.",
                Data = new { text = @"content", },
                Template = @"""{{>partial}}""",
                Expected = @"""*content*""",
                Partials = new Dictionary<string, string> { { @"partial", @"*{{text}}*" } }
            },
            new SpecTest {
                Name = @"Recursion",
                Desc = @"The greater-than operator should properly recurse.",
                Data = new RecusionTestClass { content = @"X", nodes = new RecusionTestClass[] { new RecusionTestClass { content = @"Y",  nodes = Array.Empty<RecusionTestClass>(), } }, },
                Template = @"{{>node}}",
                Expected = @"X<Y<>>",
                Partials = new Dictionary<string, string> { { @"node", @"{{content}}<{{#nodes}}{{>node}}{{/nodes}}>" } }
            },
            new SpecTest {
                Name = @"Surrounding Whitespace",
                Desc = @"The greater-than operator should not alter surrounding whitespace.",
                Data = new { },
                Template = @"| {{>partial}} |",
                Expected = @"| 	|	 |",
                Partials = new Dictionary<string, string> { { @"partial", @"	|	" } }
            },
            new SpecTest {
                Name = @"Inline Indentation",
                Desc = @"Whitespace should be left untouched.",
                Data = new { data = @"|", },
                Template = @"  {{data}}  {{> partial}}
",
                Expected = @"  |  >
>
",
                Partials = new Dictionary<string, string> { { @"partial", @">
>" } }
            },
            new SpecTest {
                Name = @"Standalone Line Endings",
                Desc = @"""\r\n"" should be considered a newline for standalone tags.",
                Data = new { },
                Template = @"|
{{>partial}}
|",
                Expected = @"|
>|",
                Partials = new Dictionary<string, string> { { @"partial", @">" } }
            },
            new SpecTest {
                Name = @"Standalone Without Previous Line",
                Desc = @"Standalone tags should not require a newline to precede them.",
                Data = new { },
                Template = @"  {{>partial}}
>",
                Expected = @"  >
  >>",
                Partials = new Dictionary<string, string> { { @"partial", @">
>" } }
            },
            new SpecTest {
                Name = @"Standalone Without Newline",
                Desc = @"Standalone tags should not require a newline to follow them.",
                Data = new { },
                Template = @">
  {{>partial}}",
                Expected = @">
  >
  >",
                Partials = new Dictionary<string, string> { { @"partial", @">
>" } }
            },
            new SpecTest {
                Name = @"Standalone Indentation",
                Desc = @"Each line of the partial should be indented before rendering.",
                Data = new { content = @"<
->", },
                Template = @"\
 {{>partial}}
/
",
                Expected = @"\
 |
 <
->
 |
/
",
                Partials = new Dictionary<string, string> { { @"partial", @"|
{{{content}}}
|
" } }
            },
            new SpecTest {
                Name = @"Padding Whitespace",
                Desc = @"Superfluous in-tag whitespace should be ignored.",
                Data = new { boolean = true, },
                Template = @"|{{> partial }}|",
                Expected = @"|[]|",
                Partials = new Dictionary<string, string> { { @"partial", @"[]" } }
            },
        }
        .Select(s => new object[] { s });

        internal class RecusionTestClass
        {
            public RecusionTestClass[] nodes;
            public string content;
        }
    }
}
