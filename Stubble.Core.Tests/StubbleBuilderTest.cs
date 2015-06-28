using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Stubble.Core.Tests
{
    public class StubbleBuilderTest
    {
        [Fact]
        public void It_Can_Add_Add_Value_Getters()
        {
            var builder = (StubbleBuilder)new StubbleBuilder()
                                .AddValueGetter(typeof (string), (o, s) => null);

            Assert.Contains(typeof(string), builder.ValueGetters.Keys);
        }

        [Fact]
        public void It_Can_Build_Stubble_Instance()
        {
            var stubble = new StubbleBuilder().Build();

            Assert.NotNull(stubble);
            Assert.NotNull(stubble.Writer.ValueRegistry);
            Assert.NotEmpty(stubble.Writer.ValueRegistry);
        }
    }
}
