// <copyright file="DevTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Dev.Parser;
using Stubble.Core.Dev.Tags;
using Xunit;
using Xunit.Abstractions;

namespace Stubble.Core.Tests
{
    public class ParserTestState
    {
        internal readonly ITestOutputHelper OutputStream;

        public ParserTestState(ITestOutputHelper outputStream)
        {
            OutputStream = outputStream;
        }

        public static IEnumerable<object[]> TemplateParsingData()
        {
            return new[]
            {
                new { index = 1, name="", arguments = new List<MustacheTag>() },
                new
                {
                    index = 2, name="{{hi}}", arguments = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = "hi", TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 4, TagEndPosition = 6, EscapeResult = true, IsClosed = true }
                    }
                },
                new
                {
                    index = 3, name="{{hi.world}}", arguments = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = "hi.world", TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 10, TagEndPosition = 12, EscapeResult = true, IsClosed = true }
                    }
                },
                new
                {
                    index = 4, name="{{hi . world}}", arguments = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = "hi . world", TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 12, TagEndPosition = 14, EscapeResult = true, IsClosed = true }
                    }
                },
                new
                {
                    index = 5, name="{{ hi}}", arguments = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = "hi", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true }
                    }
                },
                new
                {
                    index = 6, name="{{hi }}", arguments = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = "hi", TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 4, TagEndPosition = 7, EscapeResult = true, IsClosed = true }
                    }
                },
                new
                {
                    index = 7, name="{{ hi }}", arguments = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = "hi", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 8, EscapeResult = true, IsClosed = true }
                    }
                },
                new
                {
                    index = 8, name="{{{hi}}}", arguments = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = "hi", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 8, EscapeResult = false, IsClosed = true }
                    }
                },
                new
                {
                    index = 9, name="{{!hi}}", arguments = new List<MustacheTag>
                    {
                        new CommentTag { Content = "hi", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, IsClosed = true }
                    }
                },
                new
                {
                    index = 10, name="{{! hi}}", arguments = new List<MustacheTag>
                    {
                        new CommentTag { Content = " hi", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 6, TagEndPosition = 8, IsClosed = true }
                    }
                },
                new
                {
                    index = 11, name="{{! hi }}", arguments = new List<MustacheTag>
                    {
                        new CommentTag { Content = " hi ", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 7, TagEndPosition = 9, IsClosed = true }
                    }
                },
                new
                {
                    index = 12, name="{{ !hi}}", arguments = new List<MustacheTag>
                    {
                        new CommentTag { Content = "hi", TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 6, TagEndPosition = 8, IsClosed = true }
                    }
                },
                new
                {
                    index = 13, name="{{ ! hi}}", arguments = new List<MustacheTag>
                    {
                        new CommentTag { Content = " hi", TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 7, TagEndPosition = 9, IsClosed = true }
                    }
                },
                new
                {
                    index = 14, name="{{ ! hi }}", arguments = new List<MustacheTag>
                    {
                        new CommentTag { Content = " hi ", TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 8, TagEndPosition = 10, IsClosed = true }
                    }
                },
                new
                {
                    index = 15, name="a\n b", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n b", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 4, TagEndPosition = 4, IsClosed = true }
                    }
                },
                new
                {
                    index = 16, name="a{{hi}}", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 1, TagEndPosition = 1, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 1, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true },
                    }
                },
                new
                {
                    index = 17, name="a {{hi}}", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a ", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 2, ContentStartPosition = 4, ContentEndPosition = 6, TagEndPosition = 8, EscapeResult = true, IsClosed = true },
                    }
                },
                new
                {
                    index = 18, name=" a{{hi}}", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = " a", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 2, ContentStartPosition = 4, ContentEndPosition = 6, TagEndPosition = 8, EscapeResult = true, IsClosed = true },
                    }
                },
                new
                {
                    index = 19, name=" a {{hi}}", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = " a ", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 3, TagEndPosition = 3, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 3, ContentStartPosition = 5, ContentEndPosition = 7, TagEndPosition = 9, EscapeResult = true, IsClosed = true },
                    }
                },
                new
                {
                    index = 20, name="a{{hi}}b", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 1, TagEndPosition = 1, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 1, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 7, ContentStartPosition = 7, ContentEndPosition = 8, TagEndPosition = 8, IsClosed = true },
                    }
                },
                new
                {
                    index = 21, name="a{{hi}} b", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 1, TagEndPosition = 1, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 1, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true },
                        new LiteralTag { Content = " b", TagStartPosition = 7, ContentStartPosition = 7, ContentEndPosition = 9, TagEndPosition = 9, IsClosed = true },
                    }
                },
                new
                {
                    index = 22, name="a{{hi}}b ", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 1, TagEndPosition = 1, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 1, ContentStartPosition = 3, ContentEndPosition = 5, TagEndPosition = 7, EscapeResult = true, IsClosed = true },
                        new LiteralTag { Content = "b ", TagStartPosition = 7, ContentStartPosition = 7, ContentEndPosition = 9, TagEndPosition = 9, IsClosed = true },
                    }
                },
                new
                {
                    index = 23, name="a\n{{hi}} b \n", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 2, ContentStartPosition = 4, ContentEndPosition = 6, TagEndPosition = 8, EscapeResult = true, IsClosed = true },
                        new LiteralTag { Content = " b \n", TagStartPosition = 8, ContentStartPosition = 8, ContentEndPosition = 12, TagEndPosition = 12, IsClosed = true },
                    }
                },
                new
                {
                    index = 24, name="a\n {{hi}} \nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n ", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 3, TagEndPosition = 3, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 3, ContentStartPosition = 5, ContentEndPosition = 7, TagEndPosition = 9, EscapeResult = true, IsClosed = true },
                        new LiteralTag { Content = " \nb", TagStartPosition = 9, ContentStartPosition = 9, ContentEndPosition = 12, TagEndPosition = 12, IsClosed = true },
                    }
                },
                new
                {
                    index = 25, name="a\n {{!hi}} \nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new CommentTag { Content = "hi", TagStartPosition = 3, ContentStartPosition = 6, ContentEndPosition = 8, TagEndPosition = 10, IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 12, ContentStartPosition = 12, ContentEndPosition = 13, TagEndPosition = 13, IsClosed = true },
                    }
                },
                new
                {
                    index = 26, name="a\n{{#a}}{{/a}}\nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag { SectionName = "a", StartPosition = 2, EndPosition = 14, Children = new List<MustacheTag>(), IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 15, ContentStartPosition = 15, ContentEndPosition = 16, TagEndPosition = 16, IsClosed = true },
                    }
                },
                new
                {
                    index = 27, name="a\n {{#a}}{{/a}}\nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag { SectionName = "a", StartPosition = 3, EndPosition = 15, Children = new List<MustacheTag>(), IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 16, ContentStartPosition = 16, ContentEndPosition = 17, TagEndPosition = 17, IsClosed = true },
                    }
                },
                new
                {
                    index = 28, name="a\n {{#a}}{{/a}} \nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag { SectionName = "a", StartPosition = 3, EndPosition = 15, Children = new List<MustacheTag>(), IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 17, ContentStartPosition = 17, ContentEndPosition = 18, TagEndPosition = 18, IsClosed = true },
                    }
                },
                new
                {
                    index = 29, name="a\n{{#a}}\n{{/a}}\nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag { SectionName = "a", StartPosition = 2, EndPosition = 15, Children = new List<MustacheTag>(), IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 16, ContentStartPosition = 16, ContentEndPosition = 17, TagEndPosition = 17, IsClosed = true },
                    }
                },
                new
                {
                    index = 30, name="a\n {{#a}}\n{{/a}}\nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag { SectionName = "a", StartPosition = 3, EndPosition = 16, Children = new List<MustacheTag>(), IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 17, ContentStartPosition = 17, ContentEndPosition = 18, TagEndPosition = 18, IsClosed = true },
                    }
                },
                new
                {
                    index = 31, name="a\n {{#a}}\n{{/a}} \nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag { SectionName = "a", StartPosition = 3, EndPosition = 16, Children = new List<MustacheTag>(), IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 18, ContentStartPosition = 18, ContentEndPosition = 19, TagEndPosition = 19, IsClosed = true },
                    }
                },
                new
                {
                    index = 32, name="a\n{{#a}}\n{{/a}}\n{{#b}}\n{{/b}}\nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag { SectionName = "a", StartPosition = 2, EndPosition = 15, Children = new List<MustacheTag>(), IsClosed = true },
                        new SectionTag { SectionName = "b", StartPosition = 16, EndPosition = 29, Children = new List<MustacheTag>(), IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 30, ContentStartPosition = 30, ContentEndPosition = 31, TagEndPosition = 31, IsClosed = true },
                    }
                },
                new
                {
                    index = 33, name="a\n {{#a}}\n{{/a}}\n{{#b}}\n{{/b}}\nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag { SectionName = "a", StartPosition = 3, EndPosition = 16, Children = new List<MustacheTag>(), IsClosed = true },
                        new SectionTag { SectionName = "b", StartPosition = 17, EndPosition = 30, Children = new List<MustacheTag>(), IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 31, ContentStartPosition = 31, ContentEndPosition = 32, TagEndPosition = 32, IsClosed = true },
                    }
                },
                new
                {
                    index = 34, name="a\n {{#a}}\n{{/a}}\n{{#b}}\n{{/b}} \nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag { SectionName = "a", StartPosition = 3, EndPosition = 16, Children = new List<MustacheTag>(), IsClosed = true },
                        new SectionTag { SectionName = "b", StartPosition = 17, EndPosition = 30, Children = new List<MustacheTag>(), IsClosed = true },
                        new LiteralTag { Content = "b", TagStartPosition = 32, ContentStartPosition = 32, ContentEndPosition = 33, TagEndPosition = 33, IsClosed = true },
                    }
                },
                new
                {
                    index = 35, name="a\n{{#a}}\n{{#b}}\n{{/b}}\n{{/a}}\nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag
                        {
                            SectionName = "a", StartPosition = 2, EndPosition = 29, IsClosed = true, Children = new List<MustacheTag>
                            {
                                new SectionTag { SectionName = "b", StartPosition = 9, EndPosition = 22, Children = new List<MustacheTag>(), IsClosed = true },
                            }
                        },
                        new LiteralTag { Content = "b", TagStartPosition = 30, ContentStartPosition = 30, ContentEndPosition = 31, TagEndPosition = 31, IsClosed = true },
                    }
                },
                new
                {
                    index = 36, name="a\n {{#a}}\n{{#b}}\n{{/b}}\n{{/a}}\nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag
                        {
                            SectionName = "a", StartPosition = 3, EndPosition = 30, IsClosed = true, Children = new List<MustacheTag>
                            {
                                new SectionTag { SectionName = "b", StartPosition = 10, EndPosition = 23, Children = new List<MustacheTag>(), IsClosed = true },
                            }
                        },
                        new LiteralTag { Content = "b", TagStartPosition = 31, ContentStartPosition = 31, ContentEndPosition = 32, TagEndPosition = 32, IsClosed = true },
                    }
                },
                new
                {
                    index = 37, name="a\n {{#a}}\n{{#b}}\n{{/b}}\n{{/a}} \nb", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "a\n", TagStartPosition = 0, ContentStartPosition = 0, ContentEndPosition = 2, TagEndPosition = 2, IsClosed = true },
                        new SectionTag
                        {
                            SectionName = "a", StartPosition = 3, EndPosition = 30, IsClosed = true, Children = new List<MustacheTag>
                            {
                                new SectionTag { SectionName = "b", StartPosition = 10, EndPosition = 23, Children = new List<MustacheTag>(), IsClosed = true },
                            }
                        },
                        new LiteralTag { Content = "b", TagStartPosition = 32, ContentStartPosition = 32, ContentEndPosition = 33, TagEndPosition = 33, IsClosed = true },
                    }
                },
                new
                {
                    index = 38, name="{{>abc}}", arguments = new List<MustacheTag>
                    {
                        new PartialTag { Content = "abc", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 6, TagEndPosition = 8, IsClosed = true }
                    }
                },
                new
                {
                    index = 39, name="{{> abc }}", arguments = new List<MustacheTag>
                    {
                        new PartialTag { Content = "abc", TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 7, TagEndPosition = 10, IsClosed = true }
                    }
                },
                new
                {
                    index = 40, name="{{ > abc }}", arguments = new List<MustacheTag>
                    {
                        new PartialTag { Content = "abc", TagStartPosition = 0, ContentStartPosition = 5, ContentEndPosition = 8, TagEndPosition = 11, IsClosed = true }
                    }
                },
                new
                {
                    index = 41, name="{{=<% %>=}}", arguments = new List<MustacheTag>
                    {
                        new DelimiterTag { StartTag = "<%", EndTag = "%>", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 8, TagEndPosition = 11, IsClosed = true }
                    }
                },
                new
                {
                    index = 42, name="{{= <% %> =}}", arguments = new List<MustacheTag>
                    {
                        new DelimiterTag { StartTag = "<%", EndTag = "%>", TagStartPosition = 0, ContentStartPosition = 4, ContentEndPosition = 9, TagEndPosition = 13, IsClosed = true }
                    }
                },
                new
                {
                    index = 43, name="{{=<% %>=}}<%={{ }}=%>", arguments = new List<MustacheTag>
                    {
                        new DelimiterTag { StartTag = "<%", EndTag = "%>", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 8, TagEndPosition = 11, IsClosed = true },
                        new DelimiterTag { StartTag = "{{", EndTag = "}}", TagStartPosition = 11, ContentStartPosition = 14, ContentEndPosition = 19, TagEndPosition = 22, IsClosed = true },
                    }
                },
                new
                {
                    index = 44, name="{{=<% %>=}}<%hi%>", arguments = new List<MustacheTag>
                    {
                        new DelimiterTag { StartTag = "<%", EndTag = "%>", TagStartPosition = 0, ContentStartPosition = 3, ContentEndPosition = 8, TagEndPosition = 11, IsClosed = true },
                        new InterpolationTag { Content = "hi", TagStartPosition = 11, ContentStartPosition = 13, ContentEndPosition = 15, TagEndPosition = 17, EscapeResult = true, IsClosed = true }
                    }
                },
                new
                {
                    index = 45, name="{{#a}}{{/a}}hi{{#b}}{{/b}}\n", arguments = new List<MustacheTag>
                    {
                        new SectionTag { SectionName = "a", StartPosition = 0, EndPosition = 12, IsClosed = true, Children = new List<MustacheTag>() },
                        new LiteralTag { Content = "hi", TagStartPosition = 12, ContentStartPosition = 12, ContentEndPosition = 14, TagEndPosition = 14, IsClosed = true },
                        new SectionTag { SectionName = "b", StartPosition = 14, EndPosition = 26, IsClosed = true, Children = new List<MustacheTag>() },
                        new LiteralTag { Content = "\n", TagStartPosition = 26, ContentStartPosition = 26, ContentEndPosition = 27, TagEndPosition = 27, IsClosed = true },
                    }
                },
                new
                {
                    index = 46, name="{{a}}\n{{b}}\n\n{{#c}}\n{{/c}}\n", arguments = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = "a", TagStartPosition = 0, ContentStartPosition = 2, ContentEndPosition = 3, TagEndPosition = 5, EscapeResult = true, IsClosed = true },
                        new LiteralTag { Content = "\n", TagStartPosition = 5, ContentStartPosition = 5, ContentEndPosition = 6, TagEndPosition = 6, IsClosed = true },
                        new InterpolationTag { Content = "b", TagStartPosition = 6, ContentStartPosition = 8, ContentEndPosition = 9, TagEndPosition = 11, EscapeResult = true, IsClosed = true },
                        new LiteralTag { Content = "\n\n", TagStartPosition = 11, ContentStartPosition = 11, ContentEndPosition = 13, TagEndPosition = 13, IsClosed = true },
                        new SectionTag { SectionName = "c", StartPosition = 13, EndPosition = 26, IsClosed = true, Children = new List<MustacheTag>() },
                    }
                },
                new
                {
                    index = 47, name="{{#foo}}\n  {{#a}}\n    {{b}}\n  {{/a}}\n{{/foo}}\n", arguments = new List<MustacheTag>
                    {
                        new SectionTag
                        {
                            SectionName = "foo", StartPosition = 0, EndPosition = 45, IsClosed = true, Children = new List<MustacheTag>
                            {
                                new SectionTag
                                {
                                    SectionName = "a", StartPosition = 11, EndPosition = 36, IsClosed = true, Children = new List<MustacheTag>
                                    {
                                        new LiteralTag { Content = "    ", TagStartPosition = 18, ContentStartPosition = 18, ContentEndPosition = 22, TagEndPosition = 22, IsClosed = true },
                                        new InterpolationTag { Content = "b", TagStartPosition = 22, ContentStartPosition = 24, ContentEndPosition = 25, TagEndPosition = 27, EscapeResult = true, IsClosed = true },
                                        new LiteralTag { Content = "\n", TagStartPosition = 27, ContentStartPosition = 27, ContentEndPosition = 28, TagEndPosition = 28, IsClosed = true },
                                    }
                                }
                            }
                        },
                    }
                },
                new
                {
                    index = 48, name="{{#foo}}\r\n  {{#a}}\r\n    {{b}}\r\n  {{/a}}\r\n{{/foo}}\r\n", arguments = new List<MustacheTag>
                    {
                        new SectionTag
                        {
                            SectionName = "foo", StartPosition = 0, EndPosition = 49, IsClosed = true, Children = new List<MustacheTag>
                            {
                                new SectionTag
                                {
                                    SectionName = "a", StartPosition = 12, EndPosition = 39, IsClosed = true, Children = new List<MustacheTag>
                                    {
                                        new LiteralTag { Content = "    ", TagStartPosition = 20, ContentStartPosition = 20, ContentEndPosition = 24, TagEndPosition = 24, IsClosed = true },
                                        new InterpolationTag { Content = "b", TagStartPosition = 24, ContentStartPosition = 26, ContentEndPosition = 27, TagEndPosition = 29, EscapeResult = true, IsClosed = true },
                                        new LiteralTag { Content = "\r\n", TagStartPosition = 29, ContentStartPosition = 29, ContentEndPosition = 31, TagEndPosition = 31, IsClosed = true },
                                    }
                                }
                            }
                        },
                    }
                },
                new
                {
                    index = 49, name="{{#a}}a\n b{{/a}}", arguments = new List<MustacheTag>
                    {
                        new SectionTag
                        {
                            SectionName = "a", StartPosition = 0, EndPosition = 16, IsClosed = true, Children = new List<MustacheTag>
                            {
                                new LiteralTag { Content = "a\n b", TagStartPosition = 6, ContentStartPosition = 6, ContentEndPosition = 10, TagEndPosition = 10, IsClosed = true }
                            }
                        }
                    }
                },
                new
                {
                    index = 50, name="Sup {{😺}}", arguments = new List<MustacheTag>
                    {
                        new LiteralTag { Content = "Sup ", ContentStartPosition = 0, TagStartPosition = 0, ContentEndPosition = 4, TagEndPosition = 4, IsClosed = true },
                        new InterpolationTag { Content = "😺", TagStartPosition = 4, ContentStartPosition = 6, ContentEndPosition = 8, TagEndPosition = 10, EscapeResult = true, IsClosed = true }
                    }
                },
                new
                {
                    index = 51, name="{{^a}}a\n b{{/a}}", arguments = new List<MustacheTag>
                    {
                        new InvertedSectionTag
                        {
                            SectionName = "a", StartPosition = 0, EndPosition = 16, IsClosed = true, Children = new List<MustacheTag>
                            {
                                new LiteralTag { Content = "a\n b", TagStartPosition = 6, ContentStartPosition = 6, ContentEndPosition = 10, TagEndPosition = 10, IsClosed = true }
                            }
                        }
                    }
                },
            }.Select(x => new[] { new TestData { Index = x.index, Name = x.name, Arguments = x.arguments } });
        }

        [Theory]
        [MemberData(nameof(TemplateParsingData))]
        public void It_Knows_How_To_Parse(TestData data)
        {
            OutputStream.WriteLine($"Index: {data.Index}, Template: '{data.Name}'");
            var results = MustacheParser.Parse(data.Name);
            Assert.Equal(data.Arguments.Count, results.Children.Count);

            for (var i = 0; i < results.Children.Count; i++)
            {
                Assert.StrictEqual(data.Arguments[i], results.Children[i]);
            }
        }

        [Fact]
        public void It_Knows_When_There_Is_An_Unclosed_Tag()
        {
            var ex = Assert.Throws<StubbleException>(() => { MustacheParser.Parse("My name is {{name"); });
            Assert.Equal("Unclosed Tag at 17", ex.Message);
        }

        [Fact]
        public void It_Knows_When_There_Is_An_Unclosed_Section()
        {
            var ex = Assert.Throws<StubbleException>(() => { MustacheParser.Parse("A list: {{#people}}{{name}}"); });
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
        public void It_Errors_When_Given_Invalid_Tags()
        {
            var ex = Assert.Throws<StubbleException>(() => { MustacheParser.Parse("A template <% name %>", new Classes.Tags(new[] { "<%" })); });
            Assert.Equal("Invalid Tags", ex.Message);
        }

        [Fact]
        public void It_Errors_When_The_Template_Contains_Invalid_Tags()
        {
            var ex = Assert.Throws<StubbleException>(() => { MustacheParser.Parse("A template {{=<%=}}", new Classes.Tags(new[] { "<%" })); });
            Assert.Equal("Invalid Tags", ex.Message);
        }
    }

    public class TestData
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public List<MustacheTag> Arguments { get; set; }

        public override string ToString()
        {
            return $"ID #{Index} : {Name}";
        }
    }
}
