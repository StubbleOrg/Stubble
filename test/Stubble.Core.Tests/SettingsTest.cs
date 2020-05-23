using System;
using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Classes;
using Xunit;
using Stubble.Core.Settings;
using Stubble.Core.Loaders;
using Stubble.Core.Parser;
using Stubble.Core.Parser.TokenParsers;
using System.Collections;
using FluentAssertions;

namespace Stubble.Core.Tests
{
    public class SettingsTest
    {
        [Fact]
        public void It_Can_Create_Default_Settings()
        {
            var settings = new RendererSettingsBuilder().BuildSettings();

            Assert.NotEmpty(settings.ValueGetters);
            Assert.Empty(settings.TruthyChecks);
            Assert.IsType<StringLoader>(settings.TemplateLoader);
            Assert.Null(settings.PartialTemplateLoader);
            Assert.Equal(256u, settings.MaxRecursionDepth);

            var renderDefaults = RenderSettings.GetDefaultRenderSettings();
            Assert.Equal(renderDefaults.SkipRecursiveLookup, settings.RenderSettings.SkipRecursiveLookup);
            Assert.Equal(renderDefaults.ThrowOnDataMiss, settings.RenderSettings.ThrowOnDataMiss);
            Assert.Empty(settings.EnumerationConverters);
            Assert.False(settings.IgnoreCaseOnKeyLookup);
            Assert.IsType<CachedMustacheParser>(settings.Parser);
            Assert.NotNull(settings.RendererPipeline);
            Assert.Equal(new Tags("{{", "}}"), settings.DefaultTags);
            Assert.NotNull(settings.ParserPipeline);
            Assert.NotEmpty(settings.SectionBlacklistTypes);
        }

        [Fact]
        public void It_Can_Override_Parser()
        {
            var parser = new InstanceMustacheParser();
            var settings = new RendererSettingsBuilder()
                .SetMustacheParser(parser)
                .BuildSettings();

            Assert.IsType<InstanceMustacheParser>(settings.Parser);
            Assert.Equal(parser, settings.Parser);
        }

        [Fact]
        public void It_Can_Override_DefaultTags()
        {
            var tags = new Tags(":|", "|:");
            var settings = new RendererSettingsBuilder()
                .SetDefaultTags(tags)
                .BuildSettings();

            Assert.Equal(tags, settings.DefaultTags);
        }

        [Fact]
        public void It_Can_Configure_ParserPipeline()
        {
            var settings = new RendererSettingsBuilder()
                .ConfigureParserPipeline(pipe => pipe
                    .Remove<InterpolationTagParser>()
                    .Remove<PartialTagParser>())
                .BuildSettings();

            Assert.Equal(
                new[] { typeof(CommentTagParser), typeof(DelimiterTagParser) },
                settings.ParserPipeline.InlineParsers.Select(x => x.GetType()));
        }

        [Fact]
        public void It_Can_Override_Section_Blacklist()
        {
            var set = new HashSet<Type> { typeof(FactAttribute) };

            var settings = new RendererSettingsBuilder()
                .SetSectionBlacklistTypes(set)
                .BuildSettings();

            Assert.NotEmpty(settings.SectionBlacklistTypes);
            Assert.Equal(typeof(FactAttribute), settings.SectionBlacklistTypes.First());
        }

        [Fact]
        public void It_Can_Add_To_Section_Blacklist()
        {
            var settings = new RendererSettingsBuilder()
                .AddSectionBlacklistType(typeof(FactAttribute))
                .BuildSettings();

            Assert.NotEmpty(settings.SectionBlacklistTypes);
            Assert.Contains(typeof(FactAttribute), settings.SectionBlacklistTypes);
            Assert.Contains(typeof(string), settings.SectionBlacklistTypes);
            Assert.Contains(typeof(IDictionary), settings.SectionBlacklistTypes);
        }

        [Fact]
        public void Value_Getters_Should_Be_Ordered_Correctly()
        {
            var settings = new RendererSettingsBuilder()
                .BuildSettings();

            settings.OrderedValueGetters.Should().BeInAscendingOrder(TypeBySubclassAndAssignableImpl.Default);
        }
    }
}
