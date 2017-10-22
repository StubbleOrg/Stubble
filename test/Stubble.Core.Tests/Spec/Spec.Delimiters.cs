using System.Collections.Generic;
using System.Linq;

namespace Stubble.Core.Tests.Data
{
    public static partial class Specs
    {
        public static IEnumerable<object[]> DelimitersTests => new List<SpecTest>
        {
            new SpecTest {
                Name = @"Pair Behavior",
                Desc = @"The equals sign (used on both sides) should permit delimiter changes.",
                Data = new { text = @"Hey!", },
                Template = @"{{=<% %>=}}(<%text%>)",
                Expected = @"(Hey!)",
                Partials = null
            },
            new SpecTest {
                Name = @"Special Characters",
                Desc = @"Characters with special meaning regexen should be valid delimiters.",
                Data = new { text = @"It worked!", },
                Template = @"({{=[ ]=}}[text])",
                Expected = @"(It worked!)",
                Partials = null
            },
            new SpecTest {
                Name = @"Sections",
                Desc = @"Delimiters set outside sections should persist.",
                Data = new { section = true,  data = @"I got interpolated.", },
                Template = @"[
{{#section}}
  {{data}}
  |data|
{{/section}}

{{= | | =}}
|#section|
  {{data}}
  |data|
|/section|
]
",
                Expected = @"[
  I got interpolated.
  |data|

  {{data}}
  I got interpolated.
]
",
                Partials = null
            },
            new SpecTest {
                Name = @"Inverted Sections",
                Desc = @"Delimiters set outside inverted sections should persist.",
                Data = new { section = false,  data = @"I got interpolated.", },
                Template = @"[
{{^section}}
  {{data}}
  |data|
{{/section}}

{{= | | =}}
|^section|
  {{data}}
  |data|
|/section|
]
",
                Expected = @"[
  I got interpolated.
  |data|

  {{data}}
  I got interpolated.
]
",
                Partials = null
            },
            new SpecTest {
                Name = @"Partial Inheritence",
                Desc = @"Delimiters set in a parent template should not affect a partial.",
                Data = new { value = @"yes", },
                Template = @"[ {{>include}} ]
{{= | | =}}
[ |>include| ]
",
                Expected = @"[ .yes. ]
[ .yes. ]
",
                Partials = new Dictionary<string, string> { { @"include", @".{{value}}." } }
            },
            new SpecTest {
                Name = @"Post-Partial Behavior",
                Desc = @"Delimiters set in a partial should not affect the parent template.",
                Data = new { value = @"yes", },
                Template = @"[ {{>include}} ]
[ .{{value}}.  .|value|. ]
",
                Expected = @"[ .yes.  .yes. ]
[ .yes.  .|value|. ]
",
                Partials = new Dictionary<string, string> { { @"include", @".{{value}}. {{= | | =}} .|value|." } }
            },
            new SpecTest {
                Name = @"Surrounding Whitespace",
                Desc = @"Surrounding whitespace should be left untouched.",
                Data = new { },
                Template = @"| {{=@ @=}} |",
                Expected = @"|  |",
                Partials = null
            },
            new SpecTest {
                Name = @"Outlying Whitespace (Inline)",
                Desc = @"Whitespace should be left untouched.",
                Data = new { },
                Template = @" | {{=@ @=}}
",
                Expected = @" | 
",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Tag",
                Desc = @"Standalone lines should be removed from the template.",
                Data = new { },
                Template = @"Begin.
{{=@ @=}}
End.
",
                Expected = @"Begin.
End.
",
                Partials = null
            },
            new SpecTest {
                Name = @"Indented Standalone Tag",
                Desc = @"Indented standalone lines should be removed from the template.",
                Data = new { },
                Template = @"Begin.
  {{=@ @=}}
End.
",
                Expected = @"Begin.
End.
",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Line Endings",
                Desc = @"""\r\n"" should be considered a newline for standalone tags.",
                Data = new { },
                Template = @"|
{{= @ @ =}}
|",
                Expected = @"|
|",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Without Previous Line",
                Desc = @"Standalone tags should not require a newline to precede them.",
                Data = new { },
                Template = @"  {{=@ @=}}
=",
                Expected = @"=",
                Partials = null
            },
            new SpecTest {
                Name = @"Standalone Without Newline",
                Desc = @"Standalone tags should not require a newline to follow them.",
                Data = new { },
                Template = @"=
  {{=@ @=}}",
                Expected = @"=
",
                Partials = null
            },
            new SpecTest {
                Name = @"Pair with Padding",
                Desc = @"Superfluous in-tag whitespace should be ignored.",
                Data = new { },
                Template = @"|{{= @   @ =}}|",
                Expected = @"||",
                Partials = null
            },
        }.Select(s => new object[] { s });
    }
}
