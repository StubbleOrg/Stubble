// <copyright file="SectionTests.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using Stubble.Core.Classes;
using Stubble.Core.Imported;
using Stubble.Core.Renderers.StringRenderer;
using Stubble.Core.Renderers.StringRenderer.TokenRenderers;
using Stubble.Core.Settings;
using Stubble.Core.Tokens;
using Xunit;

namespace Stubble.Core.Tests.Renderers.StringRenderer
{
    public class SectionTests : RendererTestsBase
    {
        [Fact]
        public void It_Can_Render_Section_Tags_AsCondition()
        {
            const string result = "Bar";
            var settings = new RendererSettingsBuilder().BuildSettings();

            var context = new Context(
                new
                {
                    condition = true,
                    bar = "Bar"
                },
                settings,
                settings.RenderSettings);

            var stringRenderer = new StringRender(StreamWriter, settings.RendererPipeline);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionToken
                {
                    SectionName = "condition",
                    Children = new List<MustacheToken>
                    {
                        new InterpolationToken { Content = new StringSlice("bar") }
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
        public void ItIgnoresFalseySectionTags()
        {
            const string result = "";
            var settings = new RendererSettingsBuilder().BuildSettings();

            var context = new Context(
                new
                {
                    condition = false,
                    bar = "Bar"
                },
                settings,
                settings.RenderSettings);

            var stringRenderer = new StringRender(StreamWriter, settings.RendererPipeline);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionToken
                {
                    SectionName = "condition",
                    Children = new List<MustacheToken>
                    {
                        new InterpolationToken { Content = new StringSlice("bar") }
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
        public void It_Can_Render_IEnumerables_As_Lists()
        {
            const string result = "1 Bar\n2 Bar\n3 Bar\n4 Bar\n";
            var settings = new RendererSettingsBuilder().BuildSettings();

            var context = new Context(
                new
                {
                    list = new[]
                    {
                        new { a = 1 },
                        new { a = 2 },
                        new { a = 3 },
                        new { a = 4 }
                    },
                    bar = "Bar"
                },
                settings,
                settings.RenderSettings);

            var stringRenderer = new StringRender(StreamWriter, settings.RendererPipeline);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionToken
                {
                    SectionName = "list",
                    Children = new List<MustacheToken>
                    {
                        new InterpolationToken { Content = new StringSlice("a") },
                        new LiteralToken { Content = new [] { new StringSlice(" ") } },
                        new InterpolationToken { Content = new StringSlice("bar") },
                        new LiteralToken { Content = new [] { new StringSlice("\n") } },
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
        public void It_Can_Render_IEnumerators()
        {
            const string result = "a b c d e f g ";
            var settings = new RendererSettingsBuilder().BuildSettings();

            // Get Enumerator doesn't exist on string (netstandard 1.3)
            // will be added back in netstandard 2.0
            var enumerator = "abcdefg".ToCharArray().GetEnumerator();

            var context = new Context(
                new
                {
                    list = enumerator,
                    bar = "Bar"
                },
                settings,
                settings.RenderSettings);

            var stringRenderer = new StringRender(StreamWriter, settings.RendererPipeline);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionToken
                {
                    SectionName = "list",
                    Children = new List<MustacheToken>
                    {
                        new InterpolationToken { Content = new StringSlice(".") },
                        new LiteralToken { Content = new [] { new StringSlice(" ") } },
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
        public void It_Can_Render_LambdaTags_WithoutContext()
        {
            const string result = "1";
            var settings = new RendererSettingsBuilder().BuildSettings();

            var context = new Context(
                new
                {
                    lambda = new Func<string, object>((str) => 1),
                    bar = "Bar"
                },
                settings,
                settings.RenderSettings);

            var stringRenderer = new StringRender(StreamWriter, settings.RendererPipeline);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionToken
                {
                    SectionName = "lambda",
                    Children = new List<MustacheToken>
                    {
                        new InterpolationToken { Content = new StringSlice(".") },
                        new LiteralToken { Content = new [] { new StringSlice(" ") } },
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
        public void It_Can_Render_LambdaTags_WithContext()
        {
            const string result = "1 Bar";
            var settings = new RendererSettingsBuilder().BuildSettings();

            var context = new Context(
                new
                {
                    lambda = new Func<dynamic, string, object>((dyn, str) => $"1 {dyn.bar}"),
                    bar = "Bar"
                },
                settings,
                settings.RenderSettings);

            var stringRenderer = new StringRender(StreamWriter, settings.RendererPipeline);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionToken
                {
                    SectionName = "lambda",
                    Children = new List<MustacheToken>
                    {
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
        public void It_Can_Render_LambdaTags_UsingOriginalTemplate()
        {
            const string result = "<b>a b c Bar d e f</b>";
            var settings = new RendererSettingsBuilder().BuildSettings();

            var context = new Context(
                new
                {
                    lambda = new Func<string, object>(str => $"<b>{str}</b>"),
                    bar = "Bar"
                },
                settings,
                settings.RenderSettings);

            var stringRenderer = new StringRender(StreamWriter, settings.RendererPipeline);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionToken
                {
                    SectionName = "lambda",
                    SectionContent = new StringSlice("a b c {{bar}} d e f")
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
