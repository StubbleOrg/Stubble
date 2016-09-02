// <copyright file="ParserOutputTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Classes;
using Xunit;

namespace Stubble.Core.Tests
{
    public class ParserOutputTest
    {
        public static IEnumerable<object[]> ParserOutputComparisions()
        {
            return new[]
            {
                new object[]
                {
                    new ParserOutput { TokenType = "name" },
                    new ParserOutput { TokenType = "name2" }
                },
                new object[]
                {
                    new ParserOutput { TokenType = "name", Value = "hi" },
                    new ParserOutput { TokenType = "name", Value = "hi2" }
                },
                new object[]
                {
                    new ParserOutput { TokenType = "name", Value = "hi", Start = 0 },
                    new ParserOutput { TokenType = "name", Value = "hi", Start = 1 }
                },
                new object[]
                {
                    new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 6 },
                    new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 7 }
                },
                new object[]
                {
                    new ParserOutput { TokenType = "#", Value = "c", Start = 13, End = 19, ParentSectionEnd = 20 },
                    new ParserOutput { TokenType = "#", Value = "c", Start = 13, End = 19, ParentSectionEnd = 21 }
                },
                new object[]
                {
                    new ParserOutput
                    {
                        TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 24, ChildTokens = new List<ParserOutput>
                        {
                            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16, ParentSectionEnd = 17 }
                        }
                    },
                    new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 24, ChildTokens = null }
                },
                new object[]
                {
                    new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 24, ChildTokens = null },
                    new ParserOutput
                    {
                        TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 24, ChildTokens = new List<ParserOutput>
                        {
                            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16, ParentSectionEnd = 17 }
                        }
                    }
                },
                new object[]
                {
                    new ParserOutput
                    {
                        TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 24, ChildTokens = new List<ParserOutput>
                        {
                            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16, ParentSectionEnd = 17 }
                        }
                    },
                    new ParserOutput
                    {
                        TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 24, ChildTokens = new List<ParserOutput>
                        {
                            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16, ParentSectionEnd = 17 },
                            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16, ParentSectionEnd = 17 }
                        }
                    },
                },
                new object[]
                {
                    new ParserOutput
                    {
                        TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 24, ChildTokens = new List<ParserOutput>
                        {
                            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16, ParentSectionEnd = 17 }
                        }
                    },
                    new ParserOutput
                    {
                        TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 24, ChildTokens = new List<ParserOutput>
                        {
                            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16 }
                        }
                    },
                },
                new object[]
                {
                    new ParserOutput
                    {
                        TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 24, ChildTokens = new List<ParserOutput>
                        {
                            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16, ParentSectionEnd = 17 }
                        }
                    },
                    null
                }
            };
        }

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

        [Fact]
        public void It_Is_False_When_Compared_Against_Anything_But_ParserOutput()
        {
            var parserOutput = new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 6, ChildTokens = null };
            Assert.False(parserOutput.Equals("Test"));
        }

        [Theory]
        [MemberData("ParserOutputComparisions")]
        public void Equality_Is_Determined_Across_All_Values(ParserOutput a, ParserOutput b)
        {
            Assert.NotEqual(a, b);
        }
    }
}
