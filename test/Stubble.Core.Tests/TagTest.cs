using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;
using Xunit;
using Stubble.Core.Exceptions;

namespace Stubble.Core.Tests
{
    public class TagTest
    {
        [Fact]
        public void TagsCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Tags("{{", null));
            Assert.Throws<ArgumentNullException>(() => new Tags(null, "}}"));
        }

        [Fact]
        public void TagsCannotContainSpaces()
        {
            var ex = Assert.Throws<StubbleException>(() => new Tags("{{ ", "}}"));
            Assert.Contains("Start Tag", ex.Message);
            var ex2 = Assert.Throws<StubbleException>(() => new Tags("{{", " }}"));
            Assert.Contains("End Tag", ex2.Message);
        }

        [Fact]
        public void TagsCannotBeEmpty()
        {
            var ex = Assert.Throws<StubbleException>(() => new Tags("", "}}"));
            Assert.Contains("Start Tag", ex.Message);
            var ex2 = Assert.Throws<StubbleException>(() => new Tags("{{", ""));
            Assert.Contains("End Tag", ex2.Message);
        }

        [Fact]
        public void EqualityWorks()
        {
            var tagA = new Tags("{{", "}}");
            var tagB = new Tags(":|", "|:");
            var tagC = new Tags("{{", "}}");

            Assert.Equal(tagA, tagC);
            Assert.Equal(tagA.GetHashCode(), tagC.GetHashCode());

            Assert.True(tagA == tagC);
            Assert.True(tagA.Equals(tagA));
            Assert.True(tagA.Equals((object)tagA));

            Assert.NotEqual(tagA, tagB);
            Assert.NotEqual(null, tagA);
            Assert.NotEqual(tagC, null);

            Assert.False(tagA != tagC);
            Assert.False(tagA.Equals(null));
            Assert.False(tagA.Equals((object)null));
        }
    }
}
