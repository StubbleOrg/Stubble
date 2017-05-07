using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;
using Xunit;
using Stubble.Core.Dev.Settings;
using Stubble.Core.Classes.Loaders;
using Stubble.Core.Dev.Parser;
using Stubble.Core.Dev;

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
        public void It_Can_Override_ParserPipeline()
        {
            var pipeline = new ParserPipelineBuilder().Build();
            var settings = new RendererSettingsBuilder()
                .SetParserPipeline(pipeline)
                .BuildSettings();

            Assert.Equal(pipeline, settings.ParserPipeline);
        }
    }
}
