using System;
using System.Runtime.Serialization;

namespace Stubble.Core.Classes.Exceptions
{
    public class UnknownTemplateException : Exception
    {
        public UnknownTemplateException() { }
        public UnknownTemplateException(string message) : base(message) { }
        protected UnknownTemplateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public UnknownTemplateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
 