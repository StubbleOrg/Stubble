using McMaster.Extensions.Xunit;
using Stubble.Test.Shared.Spec;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Stubble.Core.Tests
{
    public class SpecTests
    {
        internal readonly ITestOutputHelper OutputStream;

        public SpecTests(ITestOutputHelper output)
        {
            OutputStream = output;
        }

        [Theory]
        [MemberData(nameof(Specs.SpecTestsWithLambda), MemberType = typeof(Specs))]
        [UseCulture("en-GB")]
        public void StringRendererSpecTest(SpecTest data)
        {
            OutputStream.WriteLine(data.Name);
            var stubble = new StubbleVisitorRenderer();
            var output = data.Partials != null ? stubble.Render(data.Template, data.Data, data.Partials) : stubble.Render(data.Template, data.Data);

            OutputStream.WriteLine("Expected \"{0}\", Actual \"{1}\"", data.Expected, output);
            Assert.Equal(data.Expected, output);
        }

        [Theory]
        [MemberData(nameof(Specs.SpecTestsWithLambda), MemberType = typeof(Specs))]
        [UseCulture("en-GB")]
        public async Task StringRendererSpecTest_Async(SpecTest data)
        {
            OutputStream.WriteLine(data.Name);
            var stubble = new StubbleVisitorRenderer();
            var output = await (data.Partials != null ? stubble.RenderAsync(data.Template, data.Data, data.Partials) : stubble.RenderAsync(data.Template, data.Data));

            OutputStream.WriteLine("Expected \"{0}\", Actual \"{1}\"", data.Expected, output);
            Assert.Equal(data.Expected, output);
        }
    }
}
