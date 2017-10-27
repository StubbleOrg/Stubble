// <copyright file="DevTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Exceptions;
using Stubble.Core.Imported;
using Stubble.Core.Parser;
using Stubble.Core.Tokens;
using Xunit;
using Xunit.Abstractions;
using static Stubble.Core.Helpers.SliceHelpers;

namespace Stubble.Core.Tests
{
    public class ParserTest
    {
        internal readonly ITestOutputHelper OutputStream;

        public ParserTest(ITestOutputHelper outputStream)
        {
            OutputStream = outputStream;
        }

        public static IEnumerable<object[]> TemplateParsingData()
        {
            return new TestData[]
            {
                new TestData { Index = 1, Name="", Arguments = new List<MustacheToken>() },
                new TestData
                {
                    Index = 2, Name="{{hi}}", Arguments = new List<MustacheToken>
                    {
                        new InterpolationToken { TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 4, TagEndPosition = 6, EscapeResult = true, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 3, Name="{{hi.world}}", Arguments = new List<MustacheToken>
                    {
                        new InterpolationToken { TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 10, TagEndPosition = 12, EscapeResult = true, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 4, Name="{{hi . world}}", Arguments = new List<MustacheToken>
                    {
                        new InterpolationToken { TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 12, TagEndPosition = 14, EscapeResult = true, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 5, Name="{{ hi}}", Arguments = new List<MustacheToken>
                    {
                        new InterpolationToken { TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 6, Name="{{hi }}", Arguments = new List<MustacheToken>
                    {
                        new InterpolationToken { TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 4, TagEndPosition = 7, EscapeResult = true, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 7, Name="{{ hi }}", Arguments = new List<MustacheToken>
                    {
                        new InterpolationToken { TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 8, EscapeResult = true, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 8, Name="{{{hi}}}", Arguments = new List<MustacheToken>
                    {
                        new InterpolationToken { TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 8, EscapeResult = false, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 9, Name="{{!hi}}", Arguments = new List<MustacheToken>
                    {
                        new CommentToken { TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 10, Name="{{! hi}}", Arguments = new List<MustacheToken>
                    {
                        new CommentToken { TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 6, TagEndPosition = 8, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 11, Name="{{! hi }}", Arguments = new List<MustacheToken>
                    {
                        new CommentToken { TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 7, TagEndPosition = 9, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 12, Name="{{ !hi}}", Arguments = new List<MustacheToken>
                    {
                        new CommentToken { TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 6, TagEndPosition = 8, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 13, Name="{{ ! hi}}", Arguments = new List<MustacheToken>
                    {
                        new CommentToken { TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 7, TagEndPosition = 9, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 14, Name="{{ ! hi }}", Arguments = new List<MustacheToken>
                    {
                        new CommentToken { TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 8, TagEndPosition = 10, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 15, Name="a\n b", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 4, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 16, Name="a{{hi}}", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 1, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 1, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 17, Name="a {{hi}}", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 2, ContentStartPosition = 4, ContentEndPosition = 6, TagEndPosition = 8, EscapeResult = true, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 18, Name=" a{{hi}}", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 2, ContentStartPosition = 4, ContentEndPosition = 6, TagEndPosition = 8, EscapeResult = true, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 19, Name=" a {{hi}}", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 3, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 3, ContentStartPosition = 5, ContentEndPosition = 7, TagEndPosition = 9, EscapeResult = true, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 20, Name="a{{hi}}b", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 1, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 1, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true },
                        new LiteralToken { ContentStartPosition = 7, ContentEndPosition = 8, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 21, Name="a{{hi}} b", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 1, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 1, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true },
                        new LiteralToken { ContentStartPosition = 7, ContentEndPosition = 9, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 22, Name="a{{hi}}b ", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 1, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 1, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true },
                        new LiteralToken { ContentStartPosition = 7, ContentEndPosition = 9, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 23, Name="a\n{{hi}} b \n", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 2, ContentStartPosition = 4, ContentEndPosition = 6, TagEndPosition = 8, EscapeResult = true, IsClosed = true },
                        new LiteralToken { ContentStartPosition = 8, ContentEndPosition = 12, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 24, Name="a\n {{hi}} \nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 3, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 3, ContentStartPosition = 5, ContentEndPosition = 7, TagEndPosition = 9, EscapeResult = true, IsClosed = true },
                        new LiteralToken { ContentStartPosition = 9, ContentEndPosition = 12, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 25, Name="a\n {{!hi}} \nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new CommentToken { TagStartPosition = 3, ContentStartPosition = 6, ContentEndPosition = 8, TagEndPosition = 10, IsClosed = true },
                        new LiteralToken { ContentStartPosition = 12, ContentEndPosition = 13, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 26, Name="a\n{{#a}}{{/a}}\nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken { SectionName = "a", StartPosition = 2, EndPosition = 14, Children = new List<MustacheToken>(), IsClosed = true },
                        new LiteralToken { ContentStartPosition = 15, ContentEndPosition = 16, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 27, Name="a\n {{#a}}{{/a}}\nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken { SectionName = "a", StartPosition = 3, EndPosition = 15, Children = new List<MustacheToken>(), IsClosed = true },
                        new LiteralToken { ContentStartPosition = 16, ContentEndPosition = 17, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 28, Name="a\n {{#a}}{{/a}} \nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken { SectionName = "a", StartPosition = 3, EndPosition = 15, Children = new List<MustacheToken>(), IsClosed = true },
                        new LiteralToken { ContentStartPosition = 17, ContentEndPosition = 18, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 29, Name="a\n{{#a}}\n{{/a}}\nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken { SectionName = "a", StartPosition = 2, EndPosition = 15, Children = new List<MustacheToken>(), IsClosed = true },
                        new LiteralToken { ContentStartPosition = 16, ContentEndPosition = 17, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 30, Name="a\n {{#a}}\n{{/a}}\nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken { SectionName = "a", StartPosition = 3, EndPosition = 16, Children = new List<MustacheToken>(), IsClosed = true },
                        new LiteralToken { ContentStartPosition = 17, ContentEndPosition = 18, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 31, Name="a\n {{#a}}\n{{/a}} \nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken { SectionName = "a", StartPosition = 3, EndPosition = 16, Children = new List<MustacheToken>(), IsClosed = true },
                        new LiteralToken { ContentStartPosition = 18, ContentEndPosition = 19, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 32, Name="a\n{{#a}}\n{{/a}}\n{{#b}}\n{{/b}}\nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken { SectionName = "a", StartPosition = 2, EndPosition = 15, Children = new List<MustacheToken>(), IsClosed = true },
                        new SectionToken { SectionName = "b", StartPosition = 16, EndPosition = 29, Children = new List<MustacheToken>(), IsClosed = true },
                        new LiteralToken { ContentStartPosition = 30, ContentEndPosition = 31, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 33, Name="a\n {{#a}}\n{{/a}}\n{{#b}}\n{{/b}}\nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken { SectionName = "a", StartPosition = 3, EndPosition = 16, Children = new List<MustacheToken>(), IsClosed = true },
                        new SectionToken { SectionName = "b", StartPosition = 17, EndPosition = 30, Children = new List<MustacheToken>(), IsClosed = true },
                        new LiteralToken { ContentStartPosition = 31, ContentEndPosition = 32, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 34, Name="a\n {{#a}}\n{{/a}}\n{{#b}}\n{{/b}} \nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken { SectionName = "a", StartPosition = 3, EndPosition = 16, Children = new List<MustacheToken>(), IsClosed = true },
                        new SectionToken { SectionName = "b", StartPosition = 17, EndPosition = 30, Children = new List<MustacheToken>(), IsClosed = true },
                        new LiteralToken { ContentStartPosition = 32, ContentEndPosition = 33, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 35, Name="a\n{{#a}}\n{{#b}}\n{{/b}}\n{{/a}}\nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken
                        {
                            SectionName = "a", StartPosition = 2, EndPosition = 29, IsClosed = true, Children = new List<MustacheToken>
                            {
                                new SectionToken { SectionName = "b", StartPosition = 9, EndPosition = 22, Children = new List<MustacheToken>(), IsClosed = true },
                            }
                        },
                        new LiteralToken { ContentStartPosition = 30, ContentEndPosition = 31, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 36, Name="a\n {{#a}}\n{{#b}}\n{{/b}}\n{{/a}}\nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken
                        {
                            SectionName = "a", StartPosition = 3, EndPosition = 30, IsClosed = true, Children = new List<MustacheToken>
                            {
                                new SectionToken { SectionName = "b", StartPosition = 10, EndPosition = 23, Children = new List<MustacheToken>(), IsClosed = true },
                            }
                        },
                        new LiteralToken { ContentStartPosition = 31, ContentEndPosition = 32, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 37, Name="a\n {{#a}}\n{{#b}}\n{{/b}}\n{{/a}} \nb", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 2, IsClosed = true },
                        new SectionToken
                        {
                            SectionName = "a", StartPosition = 3, EndPosition = 30, IsClosed = true, Children = new List<MustacheToken>
                            {
                                new SectionToken { SectionName = "b", StartPosition = 10, EndPosition = 23, Children = new List<MustacheToken>(), IsClosed = true },
                            }
                        },
                        new LiteralToken { ContentStartPosition = 32, ContentEndPosition = 33, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 38, Name="{{>abc}}", Arguments = new List<MustacheToken>
                    {
                        new PartialToken { TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 6, TagEndPosition = 8, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 39, Name="{{> abc }}", Arguments = new List<MustacheToken>
                    {
                        new PartialToken { TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 7, TagEndPosition = 10, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 40, Name="{{ > abc }}", Arguments = new List<MustacheToken>
                    {
                        new PartialToken { TagStartPosition = 0, ContentStartPosition = 5, ContentEndPosition = 8, TagEndPosition = 11, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 41, Name="{{=<% %>=}}", Arguments = new List<MustacheToken>
                    {
                        new DelimiterToken { StartTag = "<%", EndTag = "%>", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 8, TagEndPosition = 11, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 42, Name="{{= <% %> =}}", Arguments = new List<MustacheToken>
                    {
                        new DelimiterToken { StartTag = "<%", EndTag = "%>", TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 9, TagEndPosition = 13, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 43, Name="{{=<% %>=}}<%={{ }}=%>", Arguments = new List<MustacheToken>
                    {
                        new DelimiterToken { StartTag = "<%", EndTag = "%>", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 8, TagEndPosition = 11, IsClosed = true },
                        new DelimiterToken { StartTag = "{{", EndTag = "}}", TagStartPosition = 11, ContentStartPosition = 14, ContentEndPosition = 19, TagEndPosition = 22, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 44, Name="{{=<% %>=}}<%hi%>", Arguments = new List<MustacheToken>
                    {
                        new DelimiterToken { StartTag = "<%", EndTag = "%>", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 8, TagEndPosition = 11, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 11, ContentStartPosition = 13, ContentEndPosition = 15, TagEndPosition = 17, EscapeResult = true, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 45, Name="{{#a}}{{/a}}hi{{#b}}{{/b}}\n", Arguments = new List<MustacheToken>
                    {
                        new SectionToken { SectionName = "a", StartPosition = 0, EndPosition = 12, IsClosed = true, Children = new List<MustacheToken>() },
                        new LiteralToken { ContentStartPosition = 12, ContentEndPosition = 14, IsClosed = true },
                        new SectionToken { SectionName = "b", StartPosition = 14, EndPosition = 26, IsClosed = true, Children = new List<MustacheToken>() },
                        new LiteralToken { ContentStartPosition = 26, ContentEndPosition = 27, IsClosed = true },
                    }
                },
                new TestData
                {
                    Index = 46, Name="{{a}}\n{{b}}\n\n{{#c}}\n{{/c}}\n", Arguments = new List<MustacheToken>
                    {
                        new InterpolationToken { TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 3, TagEndPosition = 5, EscapeResult = true, IsClosed = true },
                        new LiteralToken { ContentStartPosition = 5, ContentEndPosition = 6, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 6, ContentStartPosition = 8, ContentEndPosition = 9, TagEndPosition = 11, EscapeResult = true, IsClosed = true },
                        new LiteralToken { ContentStartPosition = 11, ContentEndPosition = 13, IsClosed = true },
                        new SectionToken { SectionName = "c", StartPosition = 13, EndPosition = 26, IsClosed = true, Children = new List<MustacheToken>() },
                    }
                },
                new TestData
                {
                    Index = 47, Name="{{#foo}}\n  {{#a}}\n    {{b}}\n  {{/a}}\n{{/foo}}\n", Arguments = new List<MustacheToken>
                    {
                        new SectionToken
                        {
                            SectionName = "foo", StartPosition = 0, EndPosition = 45, IsClosed = true, Children = new List<MustacheToken>
                            {
                                new SectionToken
                                {
                                    SectionName = "a", StartPosition = 11, EndPosition = 36, IsClosed = true, Children = new List<MustacheToken>
                                    {
                                        new LiteralToken { ContentStartPosition = 18, ContentEndPosition = 22, IsClosed = true },
                                        new InterpolationToken { TagStartPosition = 22, ContentStartPosition = 24, ContentEndPosition = 25, TagEndPosition = 27, EscapeResult = true, IsClosed = true },
                                        new LiteralToken { ContentStartPosition = 27, ContentEndPosition = 28, IsClosed = true },
                                    }
                                }
                            }
                        },
                    }
                },
                new TestData
                {
                    Index = 48, Name="{{#foo}}\r\n  {{#a}}\r\n    {{b}}\r\n  {{/a}}\r\n{{/foo}}\r\n", Arguments = new List<MustacheToken>
                    {
                        new SectionToken
                        {
                            SectionName = "foo", StartPosition = 0, EndPosition = 49, IsClosed = true, Children = new List<MustacheToken>
                            {
                                new SectionToken
                                {
                                    SectionName = "a", StartPosition = 12, EndPosition = 39, IsClosed = true, Children = new List<MustacheToken>
                                    {
                                        new LiteralToken { ContentStartPosition = 20, ContentEndPosition = 24, IsClosed = true },
                                        new InterpolationToken { TagStartPosition = 24, ContentStartPosition = 26, ContentEndPosition = 27, TagEndPosition = 29, EscapeResult = true, IsClosed = true },
                                        new LiteralToken { ContentStartPosition = 29, ContentEndPosition = 31, IsClosed = true },
                                    }
                                }
                            }
                        },
                    }
                },
                new TestData
                {
                    Index = 49, Name="{{#a}}a\n b{{/a}}", Arguments = new List<MustacheToken>
                    {
                        new SectionToken
                        {
                            SectionName = "a", StartPosition = 0, EndPosition = 16, IsClosed = true, Children = new List<MustacheToken>
                            {
                                new LiteralToken { ContentStartPosition = 6, ContentEndPosition = 10, IsClosed = true }
                            }
                        }
                    }
                },
                new TestData
                {
                    Index = 50, Name="Sup {{😺}}", Arguments = new List<MustacheToken>
                    {
                        new LiteralToken { ContentStartPosition = 0, ContentEndPosition = 4, IsClosed = true },
                        new InterpolationToken { TagStartPosition = 4, ContentStartPosition = 6, ContentEndPosition = 8, TagEndPosition = 10, EscapeResult = true, IsClosed = true }
                    }
                },
                new TestData
                {
                    Index = 51, Name="{{^a}}a\n b{{/a}}", Arguments = new List<MustacheToken>
                    {
                        new InvertedSectionToken
                        {
                            SectionName = "a", StartPosition = 0, EndPosition = 16, IsClosed = true, Children = new List<MustacheToken>
                            {
                                new LiteralToken { ContentStartPosition = 6, ContentEndPosition = 10, IsClosed = true }
                            }
                        }
                    }
                }
            }
            .Select(BuildTestCase).Select(x => new[] { x });
        }

        private static TestData BuildTestCase(TestData data)
        {
            foreach (var argument in data.Arguments)
            {
                ProcessTag(argument);
            }

            void ProcessTag(MustacheToken tag)
            {
                switch (tag)
                {
                    case InlineToken inline:
                        inline.Content = new StringSlice(data.Name, inline.ContentStartPosition, inline.ContentEndPosition - 1);
                        break;
                    case LiteralToken literal:
                        literal.Content = SplitSliceToLines(
                            new StringSlice(
                                data.Name,
                                literal.ContentStartPosition,
                                literal.ContentEndPosition - 1)).ToArray();
                        break;
                    case BlockToken blockTag:
                        foreach (var child in blockTag.Children)
                        {
                            ProcessTag(child);
                        }
                        break;
                }
            }

            return data;
        }

        [Theory]
        [MemberData(nameof(TemplateParsingData))]
        public void It_Knows_How_To_Parse(TestData data)
        {
            OutputStream.WriteLine($"Index: {data.Index}, Template: '{data.Name}'");
            var results = MustacheParser.Parse(data.Name);

            Assert.Equal("Root", results.Identifier);
            Assert.Equal(data.Arguments.Count, results.Children.Count);

            for (var i = 0; i < results.Children.Count; i++)
            {
                Assert.StrictEqual(data.Arguments[i], results.Children[i]);
            }
        }

        [Fact]
        public void It_Knows_When_There_Is_An_Unclosed_Tag()
        {
            var ex = Assert.Throws<StubbleException>(() => { MustacheParser.Parse("My Name is {{Name"); });
            Assert.Equal("Unclosed Tag at 17", ex.Message);
        }

        [Fact]
        public void It_Knows_When_There_Is_An_Unclosed_Section()
        {
            var ex = Assert.Throws<StubbleException>(() => { MustacheParser.Parse("A list: {{#people}}{{Name}}"); });
            Assert.Equal("Unclosed Block 'people' at 27", ex.Message);
        }

        [Fact]
        public void It_Knows_When_There_Is_An_Unopened_Section()
        {
            var ex = Assert.Throws<StubbleException>(() => { MustacheParser.Parse("The end of the list! {{/people}}"); });
            Assert.Equal("Unopened Block 'people' at 21", ex.Message);
        }

        [Fact]
        public void It_Errors_When_You_Close_The_Wrong_Section()
        {
            var ex = Assert.Throws<StubbleException>(() => { MustacheParser.Parse("{{#Section}}Herp De Derp{{/WrongSection}}"); });
            Assert.Equal("Cannot close Block 'WrongSection' at 24. There is already an unclosed Block 'Section'", ex.Message);
        }

        [Fact]
        public void Instance_Parser_Just_Passes_Through()
        {
            var instance = new InstanceMustacheParser();
            var tags = instance.Parse("{{foo}}");
            Assert.NotNull(tags);
            Assert.Equal("Root", tags.Identifier);
            Assert.NotEmpty(tags.Children);
        }

        [Fact]
        public void Cached_Parser_Should_Cache_Template()
        {
            var instance = new CachedMustacheParser();
            var tags = instance.Parse("{{foo}}");
            Assert.NotNull(tags);
            Assert.Equal("Root", tags.Identifier);
            Assert.NotEmpty(tags.Children);

            var tags2 = instance.Parse("{{foo}}");
            Assert.NotNull(tags2);
            Assert.Equal("Root", tags2.Identifier);
            Assert.NotEmpty(tags2.Children);

            Assert.Single(instance.Cache);
        }

        [Fact]
        public void Cached_Parser_Should_Cache_Template_IncludingTags()
        {
            var instance = new CachedMustacheParser();
            var tags = instance.Parse("{{foo}}");
            Assert.NotNull(tags);
            Assert.Equal("Root", tags.Identifier);
            Assert.NotEmpty(tags.Children);
            Assert.IsType<InterpolationToken>(tags.Children[0]);

            var tags2 = instance.Parse("{{foo}}", new Classes.Tags("|{", "}|"));
            Assert.NotNull(tags2);
            Assert.Equal("Root", tags2.Identifier);
            Assert.NotEmpty(tags2.Children);
            Assert.IsType<LiteralToken>(tags2.Children[0]);

            Assert.Equal(2, instance.Cache.Count);
        }

        [Fact]
        public void Cached_Parser_Should_Throw_On_Null()
        {
            var instance = new CachedMustacheParser();
            Assert.Throws<ArgumentNullException>(() => instance.Parse(null));
        }

        [Fact]
        public void TemplateKey_Equality_Works()
        {
            var key = new CachedMustacheParser.TemplateKey("abc", null, 0);
            var key2 = new CachedMustacheParser.TemplateKey("abc", null, 0);
            Assert.False(key.Equals(null));
            Assert.Equal(key, key2);
        }

        [Fact]
        public void Static_MustacheParser_Throws_On_Null()
        {
            Assert.Throws<ArgumentNullException>(() => MustacheParser.Parse(null));
        }
    }

    public class TestData
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public List<MustacheToken> Arguments { get; set; }

        public override string ToString()
        {
            return $"ID #{Index} : {Name}";
        }
    }
}
