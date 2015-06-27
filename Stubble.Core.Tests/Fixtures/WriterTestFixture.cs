using Stubble.Core.Classes;

namespace Stubble.Core.Tests.Fixtures
{
    public class WriterTestFixture
    {
        public Writer Writer { get; set; }

        public WriterTestFixture()
        {
            Writer = new Writer();
            Writer.ValueRegistry = new Registry().ValueGetters;
        }
    }
}