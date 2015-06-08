using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;
using Xunit;

namespace Stubble.Core.Tests
{
    
    [CollectionDefinition("MyCollection")]
    public class CollectionClass : ICollectionFixture<StubbleTestFixture> { }

    [Collection("MyCollection")]
    public class StubbleTest
    {
        public static IEnumerable<object[]> TemplateParsingData = ParserTest.TemplateParsingData();
        public Stubble Stubble;

        public StubbleTest(StubbleTestFixture fixture)
        {
            Stubble = fixture.Stubble;
        }

        [Theory, MemberData("TemplateParsingData")]
        public void It_Can_Handle_Parsing_And_Caching(string template, IList<ParserOutput> result)
        {
            var results = Stubble.Parse(template);

            Assert.False(Stubble.Cache.Count > 15);
            for (var i = 0; i < results.Count; i++)
            {
                Assert.Equal(results[i], result[i]);
            }
        }
    }

    public class StubbleTestFixture
    {
        public Stubble Stubble { get; set; }

        public StubbleTestFixture()
        {
            Stubble = new Stubble();
        }
    }
}
