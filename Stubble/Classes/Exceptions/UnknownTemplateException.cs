using System;

namespace Stubble.Core.Classes.Exceptions
{
    public class UnknownTemplateException : Exception
    {
        public UnknownTemplateException() { }
        public UnknownTemplateException(string message) : base(message) { }
        public UnknownTemplateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
 