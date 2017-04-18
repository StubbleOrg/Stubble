// <copyright file="RendererTests.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Loaders;
using Stubble.Core.Dev.Imported;
using Stubble.Core.Dev.Renderers;
using Stubble.Core.Dev.Renderers.Token;
using Stubble.Core.Dev.Settings;
using Stubble.Core.Dev.Tags;
using Xunit;

namespace Stubble.Core.Tests.Renderers.StringRenderer
{
    public class RendererTests : RendererTestsBase
    {
        [Fact]
        public void It_Can_Render_LiteralTags()
        {
            StringSlice content = new StringSlice("I'm a literal tag");

            var context = new Context(
                new { },
                new RendererSettingsBuilder().BuildSettings(),
                Classes.RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var rawTokenRenderer = new LiteralTokenRenderer();
            rawTokenRenderer.Write(
                stringRenderer,
                new LiteralTag()
                {
                    Content = new[] {content}
                }, context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(content.ToString(), myStr);
        }

        [Fact]
        public void It_Can_Render_InterpolationTag_SimpleValue()
        {
            const string result = "Bar";
            var context = new Context(
                new { foo = "Bar" },
                new RendererSettingsBuilder().BuildSettings(),
                Classes.RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var interpolationTokenRenderer = new InterpolationTokenRenderer();
            interpolationTokenRenderer.Write(
                stringRenderer,
                new InterpolationTag
                {
                    Content = new StringSlice("foo"),
                    EscapeResult = false,
                }, context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_Render_InterpolationTag_Escaped_SimpleValue()
        {
            const string result = "A &amp; B";
            var context = new Context(
                new { foo = "A & B" },
                new RendererSettingsBuilder().BuildSettings(),
                Classes.RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var interpolationTokenRenderer = new InterpolationTokenRenderer();
            interpolationTokenRenderer.Write(
                stringRenderer,
                new InterpolationTag
                {
                    Content = new StringSlice("foo"),
                    EscapeResult = true,
                }, context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_Render_InterpolationTag_Lambda_Simple()
        {
            const string result = "TestyTest";
            var context = new Context(
                new { foo = new Func<string>(() => "TestyTest") },
                new RendererSettingsBuilder().BuildSettings(),
                Classes.RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var interpolationTokenRenderer = new InterpolationTokenRenderer();
            interpolationTokenRenderer.Write(
                stringRenderer,
                new InterpolationTag
                {
                    Content = new StringSlice("foo"),
                    EscapeResult = true,
                }, context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_Render_InterpolationTag_Lambda_Simple_Escaped()
        {
            const string result = "A &amp; B";
            var context = new Context(
                new { foo = new Func<string>(() => "A & B") },
                new RendererSettingsBuilder().BuildSettings(),
                Classes.RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var interpolationTokenRenderer = new InterpolationTokenRenderer();
            interpolationTokenRenderer.Write(
                stringRenderer,
                new InterpolationTag
                {
                    Content = new StringSlice("foo"),
                    EscapeResult = true,
                }, context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_Render_InterpolationTag_Lambda_Tag()
        {
            const string result = "Bar";
            var context = new Context(
                new
                {
                    foo = new Func<string>(() => "{{bar}}"),
                    bar = "Bar"
                },
                new RendererSettingsBuilder().BuildSettings(),
                Classes.RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var interpolationTokenRenderer = new InterpolationTokenRenderer();
            interpolationTokenRenderer.Write(
                stringRenderer,
                new InterpolationTag
                {
                    Content = new StringSlice("foo"),
                    EscapeResult = true,
                }, context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_RenderPartialTags()
        {
            const string result = "Bar";

            var setingsBuilder = new RendererSettingsBuilder();
            setingsBuilder.SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string>()
            {
                { "foo", "{{bar}}" }
            }));

            var context = new Context(
                new
                {
                    bar = "Bar"
                },
                setingsBuilder.BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var partialTokenRenderer = new PartialTokenRenderer();
            partialTokenRenderer.Write(
                stringRenderer,
                new PartialTag
                {
                    Content = new StringSlice("foo"),
                }, context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Ignores_Tags_It_Cant_Find()
        {
            const string result = "";

            var setingsBuilder = new RendererSettingsBuilder();
            setingsBuilder.SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string>()
            {
                { "bar", "{{bar}}" }
            }));

            var context = new Context(
                new
                {
                    bar = "Bar"
                },
                setingsBuilder.BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var partialTokenRenderer = new PartialTokenRenderer();
            partialTokenRenderer.Write(
                stringRenderer,
                new PartialTag
                {
                    Content = new StringSlice("foo"),
                }, context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Renders_Inverted_Tags_When_Falsey_Bool()
        {
            const string result = "I'm false";;

            var context = new Context(
                new
                {
                    check = false
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new InvertedSectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new InvertedSectionTag
                {
                    SectionName = "check",
                    Children = new List<MustacheTag>
                    {
                        new LiteralTag { Content = new[] { new StringSlice("I'm false") } }
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Renders_Inverted_Tags_When_Falsey_List()
        {
            const string result = "I'm also false";

            var context = new Context(
                new
                {
                    list = new object[] { }
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new InvertedSectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new InvertedSectionTag
                {
                    SectionName = "list",
                    Children = new List<MustacheTag>
                    {
                        new LiteralTag {
                            Content = new []
                            {
                                new StringSlice("I'm also false")
                            }
                        }
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Renders_Inverted_Tags_When_Value_Doesnt_Exist()
        {
            const string result = "I'm also also false";

            var context = new Context(
                new
                {
                    check = false
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new InvertedSectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new InvertedSectionTag
                {
                    SectionName = "notfound",
                    Children = new List<MustacheTag>
                    {
                        new LiteralTag {
                            Content = new []
                            {
                                new StringSlice("I'm also also false")
                            }
                        }
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Doesnt_Render_Inverted_Tags_When_True()
        {
            const string result = "";

            var context = new Context(
                new
                {
                    check = true
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new InvertedSectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new InvertedSectionTag
                {
                    SectionName = "check",
                    Children = new List<MustacheTag>
                    {
                        new LiteralTag
                        {
                            Content = new []
                            {
                                new StringSlice("I'm not displayed")
                            }
                        }
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }
    }
}
