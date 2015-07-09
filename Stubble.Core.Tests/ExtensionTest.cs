using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Helpers;
using Xunit;

namespace Stubble.Core.Tests
{
    public class ExtensionTest
    {
        [Fact]
        public void It_Can_Slice_Strings()
        {
            var sliced = "I'm a String".Slice(0, 3);
            Assert.Equal("I'm", sliced);
            var slicedAgain = "I'm a String".Slice(0, -7);
            Assert.Equal("I'm a", slicedAgain);
        }

        [Fact]
        public void It_Is_Truthy_For_Strings()
        {
            Assert.True(ValueHelpers.IsTruthy("Yes"));
            Assert.False(ValueHelpers.IsTruthy(""));
        }
    }
}
