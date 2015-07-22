using System;

namespace Stubble.Core.Classes.Exceptions
{
    public class StubbleDataMissException : Exception
    {
        public StubbleDataMissException() { }
        public StubbleDataMissException(string message) : base(message) { }
        public StubbleDataMissException(string message, Exception innerException) : base(message, innerException) { }
    }
}
