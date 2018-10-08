using Stubble.Core.Builders;
using Stubble.Core.Imported;
using Stubble.Core.Parser;
using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Parser.TokenParsers;
using Stubble.Core.Tokens;
using Xunit;

namespace Stubble.Core.Tests
{
    public class ParserPipelineBuilderTest
    {
        [Fact]
        public void It_Should_Be_Able_To_Remove_InlineParsers()
        {
            var builder = new ParserPipelineBuilder();
            builder.Remove<InterpolationTagParser>();
            var pipeline = builder.Build();

            Assert.DoesNotContain(pipeline.InlineParsers, p => p is InterpolationTagParser);
        }

        [Fact]
        public void It_Should_Be_Able_To_Remove_BlockParsers()
        {
            var builder = new ParserPipelineBuilder();
            builder.Remove<SectionTagParser>();
            var pipeline = builder.Build();

            Assert.DoesNotContain(pipeline.BlockParsers, p => p is SectionTagParser);
        }

        [Fact]
        public void It_Should_Be_Able_To_Replace_InlineParsers()
        {
            var builder = new ParserPipelineBuilder();
            builder.Replace<InterpolationTagParser>(new MyCustomInlineParser());
            var pipeline = builder.Build();

            Assert.DoesNotContain(pipeline.InlineParsers, p => p is InterpolationTagParser);
        }

        [Fact]
        public void It_Should_Be_Able_To_Replace_BlockParsers()
        {
            var builder = new ParserPipelineBuilder();
            builder.Replace<SectionTagParser>(new MyCustomBlockParser());
            var pipeline = builder.Build();

            Assert.DoesNotContain(pipeline.BlockParsers, p => p is SectionTagParser);
        }

        [Fact]
        public void It_Should_Be_Able_To_AddAfter_InlineParsers()
        {
            var builder = new ParserPipelineBuilder();
            builder.AddAfter<InterpolationTagParser>(new MyCustomInlineParser());
            var pipeline = builder.Build();

            Assert.Contains(pipeline.InlineParsers, p => p is InterpolationTagParser);
            Assert.Contains(pipeline.InlineParsers, p => p is MyCustomInlineParser);

            var interpolationIndex = pipeline.InlineParsers.FindIndex(p => p is InterpolationTagParser);
            var customIndex = pipeline.InlineParsers.FindIndex(p => p is MyCustomInlineParser);
            Assert.True(customIndex > interpolationIndex);
        }

        [Fact]
        public void It_Should_Be_Able_To_AddAfter_BlockParsers()
        {
            var builder = new ParserPipelineBuilder();
            builder.AddAfter<SectionTagParser>(new MyCustomBlockParser());
            var pipeline = builder.Build();

            Assert.Contains(pipeline.BlockParsers, p => p is SectionTagParser);
            Assert.Contains(pipeline.BlockParsers, p => p is MyCustomBlockParser);

            var sectionIndex = pipeline.BlockParsers.FindIndex(p => p is SectionTagParser);
            var customIndex = pipeline.BlockParsers.FindIndex(p => p is MyCustomBlockParser);
            Assert.True(customIndex > sectionIndex);
        }

        [Fact]
        public void It_Should_Be_Able_To_AddBefore_InlineParsers()
        {
            var builder = new ParserPipelineBuilder();
            builder.AddBefore<InterpolationTagParser>(new MyCustomInlineParser());
            var pipeline = builder.Build();

            Assert.Contains(pipeline.InlineParsers, p => p is InterpolationTagParser);
            Assert.Contains(pipeline.InlineParsers, p => p is MyCustomInlineParser);

            var interpolationIndex = pipeline.InlineParsers.FindIndex(p => p is InterpolationTagParser);
            var customIndex = pipeline.InlineParsers.FindIndex(p => p is MyCustomInlineParser);
            Assert.True(customIndex < interpolationIndex);
        }

        [Fact]
        public void It_Should_Be_Able_To_AddBefore_BlockParsers()
        {
            var builder = new ParserPipelineBuilder();
            builder.AddBefore<SectionTagParser>(new MyCustomBlockParser());
            var pipeline = builder.Build();

            Assert.Contains(pipeline.BlockParsers, p => p is SectionTagParser);
            Assert.Contains(pipeline.BlockParsers, p => p is MyCustomBlockParser);

            var sectionIndex = pipeline.BlockParsers.FindIndex(p => p is SectionTagParser);
            var customIndex = pipeline.BlockParsers.FindIndex(p => p is MyCustomBlockParser);
            Assert.True(customIndex < sectionIndex);
        }

        private class MyCustomInlineParser : InlineParser
        {
            public override bool Match(Processor processor, ref StringSlice slice)
            {
                throw new System.NotImplementedException();
            }
        }

        private class MyCustomBlockParser : BlockParser
        {
            public override void EndBlock(Processor processor, BlockToken token, BlockCloseToken closeToken, StringSlice content)
            {
                throw new System.NotImplementedException();
            }

            public override bool TryClose(Processor processor, ref StringSlice slice, BlockToken token)
            {
                throw new System.NotImplementedException();
            }

            public override ParserState TryOpenBlock(Processor processor, ref StringSlice slice)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
