using Stubble.Test.Shared.Spec;
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
        [MemberData(nameof(Specs.SpecTests), MemberType = typeof(Specs))]
        public void StringRendererSpecTest(SpecTest data)
        {
            OutputStream.WriteLine(data.Name);
            var stubble = new StubbleVisitorRenderer();
            var output = data.Partials != null ? stubble.Render(data.Template, data.Data, data.Partials) : stubble.Render(data.Template, data.Data);

            OutputStream.WriteLine("Expected \"{0}\", Actual \"{1}\"", data.Expected, output);
            Assert.Equal(data.Expected, output);
        }
    }
}
