using System.Collections.Generic;
using Stubble.Core.Classes.Loaders;
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
            Assert.Null(builder.ValueGetters[typeof(string)](null, null));
        }

        [Fact]
        public void It_Can_Add_Add_Token_Getters()
        {
            var builder = (StubbleBuilder) new StubbleBuilder()
                            .AddTokenGetter("MyToken", (s, tags) => null);

            Assert.Contains("MyToken", builder.TokenGetters.Keys);
            Assert.Null(builder.TokenGetters["MyToken"](null, null));
        }

        [Fact]
        public void It_Can_Add_Truthy_Checks()
        {
            var builder = (StubbleBuilder) new StubbleBuilder()
                .AddTruthyCheck((val) =>
                {
                    if (val is string)
                    {
                        return val.Equals("Foo");
                    }
                    return null;
                });

            Assert.Equal(1, builder.TruthyChecks.Count);
            Assert.True(builder.TruthyChecks[0]("Foo"));
            Assert.False(builder.TruthyChecks[0]("Bar"));
            Assert.Null(builder.TruthyChecks[0](null));
        }

        [Fact]
        public void It_Can_Set_Template_Loader()
        {
            var builder = (StubbleBuilder) new StubbleBuilder()
                .SetTemplateLoader(new DictionaryLoader(new Dictionary<string, string> {{"test", "{{foo}}"}}));

            Assert.NotNull(builder.TemplateLoader);
            Assert.True(builder.TemplateLoader is DictionaryLoader);
        }

        [Fact]
        public void It_Can_Set_A_Partial_Template_Loader()
        {
            var builder = (StubbleBuilder)new StubbleBuilder()
                   .SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string> { { "test", "{{foo}}" } }));

            Assert.NotNull(builder.PartialTemplateLoader);
            Assert.True(builder.PartialTemplateLoader is DictionaryLoader);
        }

        [Fact]
        public void It_Can_Build_Stubble_Instance()
        {
            var stubble = new StubbleBuilder().Build();

            Assert.NotNull(stubble);
            Assert.NotNull(stubble.Registry.ValueGetters);
            Assert.NotNull(stubble.Registry.TokenGetters);
            Assert.NotNull(stubble.Registry.TokenMatchRegex);
            Assert.NotNull(stubble.Registry.TruthyChecks);
            Assert.True(stubble.Registry.TemplateLoader is StringLoader);
            Assert.Null(stubble.Registry.PartialTemplateLoader);

            Assert.NotEmpty(stubble.Registry.ValueGetters);
            Assert.NotEmpty(stubble.Registry.TokenGetters);
        }

    }
}
