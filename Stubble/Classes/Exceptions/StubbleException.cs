using System;

namespace Stubble.Core.Classes.Exceptions
{
    public class StubbleException : Exception
    {
        public StubbleException() { }
        public StubbleException(string message) : base(message) { }
        public StubbleException(string message, Exception innerException) : base(message, innerException) { }
    }
}
