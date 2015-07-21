using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes.Exceptions
{
    public class StubbleException : Exception
    {
        public StubbleException() { }
        public StubbleException(string message) : base(message) { }
        protected StubbleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public StubbleException(string message, Exception innerException) : base(message, innerException) { }
    }
}
