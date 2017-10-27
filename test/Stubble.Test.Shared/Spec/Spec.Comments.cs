using System.Collections.Generic;
using System.Linq;

namespace Stubble.Test.Shared.Spec
{
    public static partial class Specs
    {
        public static IEnumerable<object[]> CommentTests => new List<SpecTest>
        {
            new SpecTest {
                Name = "Inline",
                Desc = "Comment blocks should be removed from the template.",
                Data = new { },
                Template = @"12345{{! Comment Block! }}67890",
                Expected = @"1234567890"
            },
            new SpecTest {
                Name = "Multiline",
                Desc = "Multiline comments should be permitted.",
                Data = new { },
                Template = @"12345{{!
  This is a
  multi-line comment...
}}67890
",
                Expected = @"1234567890
"
            },
            new SpecTest {
                Name = "Standalone",
                Desc = "All standalone comment lines should be removed.",
                Data = new { },
                Template = @"Begin.
{{! Comment Block! }}
End.
",
                Expected = @"Begin.
End.
"
            },
            new SpecTest {
                Name = "Indented Standalone",
                Desc = "All standalone comment lines should be removed.",
                Data = new { },
                Template = @"Begin.
  {{! Indented Comment Block! }}
End.
",
                Expected = @"Begin.
End.
"
            },
            new SpecTest {
                Name = "Standalone Line Endings",
                Desc = "\"\r\n\" should be considered a newline for standalone tags.",
                Data = new { },
                Template = @"|
{{! Standalone Comment }}
|",
                Expected = @"|
|"
            },
            new SpecTest {
                Name = "Standalone Without Previous Line",
                Desc = "Standalone tags should not require a newline to precede them.",
                Data = new { },
                Template = @"  {{! I'm Still Standalone }}
!",
                Expected = @"!"
            },
            new SpecTest {
                Name = "Standalone Without Newline",
                Desc = "Standalone tags should not require a newline to follow them.",
                Data = new { },
                Template = @"!
  {{! I'm Still Standalone }}",
                Expected = @"!
"
            },
            new SpecTest {
                Name = "Multiline Standalone",
                Desc = "All standalone comment lines should be removed.",
                Data = new { },
                Template = @"Begin.
{{!
Something's going on here...
}}
End.
",
                Expected = @"Begin.
End.
"
            },
            new SpecTest {
                Name = "Indented Multiline Standalone",
                Desc = "All standalone comment lines should be removed.",
                Data = new { },
                Template = @"Begin.
  {{!
    Something's going on here...
  }}
End.
",
                Expected = @"Begin.
End.
"
            },
            new SpecTest {
                Name = "Indented Inline",
                Desc = "Inline comments should not strip whitespace",
                Data = new { },
                Template = @"  12 {{! 34 }}
",
                Expected = @"  12 
"
            },
            new SpecTest {
                Name = "Surrounding Whitespace",
                Desc = "Comment removal should preserve surrounding whitespace.",
                Data = new { },
                Template = @"12345 {{! Comment Block! }} 67890",
                Expected = @"12345  67890"
            },
        }.Select(s => new object[] { s });
    }
}
