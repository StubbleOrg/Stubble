using System;
using Stubble.Core.Classes.Exceptions;
using Xunit;

namespace Stubble.Core.Tests
{
    public class ExceptionTests
    {
        [Fact]
        public void UnknownTemplateExceptions_Constructors_Should_Work()
        {
            Assert.NotNull(new UnknownTemplateException());
            Assert.NotNull(new UnknownTemplateException("Test"));
            Assert.NotNull(new UnknownTemplateException("Test", new Exception("Inner Test")));
        }
    }
}
