using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;
using Stubble.Core.Tests.Fixtures;
using Xunit;

namespace Stubble.Core.Tests
{
    [CollectionDefinition("WriterCollection")]
    public class WriterCollection : ICollectionFixture<WriterTestFixture> { }

    [Collection("WriterCollection")]
    public class WriterTest
    {
        public static IEnumerable<object[]> TemplateParsingData = ParserTest.TemplateParsingData();
        public Writer Writer;

        public WriterTest(WriterTestFixture fixture)
        {
            Writer = fixture.Writer;
        }

        [Theory, MemberData("TemplateParsingData")]
        public void It_Can_Handle_Parsing_And_Caching(string template, IList<ParserOutput> result)
        {
            var results = Writer.Parse(template);

            Assert.False(Writer.Cache.Count > 15);
            for (var i = 0; i < results.Count; i++)
            {
                Assert.StrictEqual(results[i], result[i]);
            }
        }

        [Fact]
        public void It_Can_Render_Templates()
        {
            var output = Writer.Render("{{foo}}", new { foo = "Bar" }, null);
            Assert.Equal("Bar", output);
        }
    }
}
