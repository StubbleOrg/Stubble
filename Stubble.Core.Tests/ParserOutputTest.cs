using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;
using Xunit;

namespace Stubble.Core.Tests
{
    public class ParserOutputTest
    {
        [Fact]
        public void It_Has_The_Same_Hashcode_Each_Time()
        {
            var parserOutput = new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 6 };
            var hashcode = parserOutput.GetHashCode();

            Assert.Equal(hashcode, parserOutput.GetHashCode());
        }

        [Fact]
        public void Equality_Can_Be_Determined_By_Values()
        {
            var parserOutputA = new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 6 };
            var parserOutputB = new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 6 };

            Assert.Equal(parserOutputA, parserOutputB);
        }

        [Fact]
        public void Equality_Can_Be_Determined_By_Values_Even_With_Null_Children()
        {
            var parserOutputA = new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 6, ChildTokens = null };
            var parserOutputB = new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 6, ChildTokens = null };

            Assert.Equal(parserOutputA, parserOutputB);
        }
    }
}
