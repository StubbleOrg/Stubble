using System;
using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Classes;
using Stubble.Core.Tests.Fixtures;
using Xunit;

namespace Stubble.Core.Tests
{
    [CollectionDefinition("ParserCollection")]
    public class ParserCollection : ICollectionFixture<ParserTestFixture> { }

    [Collection("ParserCollection")]
    public class ParserTest
    {
        internal Parser Parser;

        public ParserTest(ParserTestFixture fixture)
        {
            Parser = fixture.Parser;
        }

        [Theory, MemberData("TemplateParsingData")]
        public void It_Knows_How_To_Parse(string template, IList<ParserOutput> result)
        {
            var results = Parser.ParseTemplate(template);

            for (var i = 0; i < results.Count; i++)
            {
                Assert.StrictEqual(result[i], results[i]);
            }
        }

        [Fact]
        public void It_Knows_When_There_Is_An_Unclosed_Tag()
        {
            var ex = Assert.Throws<Exception>(delegate { Parser.ParseTemplate("My name is {{name"); });
            Assert.Equal("Unclosed Tag at 17", ex.Message);
        }

        [Fact]
        public void It_Knows_When_There_Is_An_Unclosed_Section()
        {
            var ex = Assert.Throws<Exception>(delegate { Parser.ParseTemplate("A list: {{#people}}{{name}}"); });
            Assert.Equal("Unclosed Section 'people' at 27", ex.Message);
        }

        [Fact]
        public void It_Knows_When_There_Is_An_Unopened_Section()
        {
            var ex = Assert.Throws<Exception>(delegate { Parser.ParseTemplate("The end of the list! {{/people}}"); });
            Assert.Equal("Unopened Section 'people' at 21", ex.Message);
        }

        [Fact]
        public void It_Errors_When_Given_Invalid_Tags()
        {
            var ex = Assert.Throws<Exception>(delegate { Parser.ParseTemplate("A template <% name %>", new Tags(new[] { "<%" })); });
            Assert.Equal("Invalid Tags", ex.Message);
        }

        [Fact]
        public void It_Errors_When_The_Template_Contains_Invalid_Tags()
        {
            var ex = Assert.Throws<Exception>(delegate { Parser.ParseTemplate("A template {{=<%=}}", new Tags(new[] { "<%" })); });
            Assert.Equal("Invalid Tags", ex.Message);
        }

        [Fact]
        public void It_Errors_When_You_Close_The_Wrong_Section()
        {
            var ex = Assert.Throws<Exception>(delegate { Parser.ParseTemplate("{{#Section}}Herp De Derp{{/wrongSection}}"); });
            Assert.Equal("Unclosed Section 'Section' at 24", ex.Message);
        }

        [Fact]
        public void It_Only_Cache_Four_Regex_Tags()
        {
            Parser.RegexCacheSize = 4;
            Parser.TagRegexCache.Clear();
            Parser.ParseTemplate("Test 1 {{=<% %>=}}");
            Assert.Equal(2, Parser.TagRegexCache.Count);
            Parser.ParseTemplate("Test 2 {{={| |}=}}");
            Assert.Equal(3, Parser.TagRegexCache.Count);
            Parser.ParseTemplate("Test 3 {{=<: :>=}}");
            Assert.Equal(4, Parser.TagRegexCache.Count);
            Parser.ParseTemplate("Test 4 {{=|# #|=}}");
            Assert.Equal(4, Parser.TagRegexCache.Count);
        }

        [Fact]
        public void It_Can_Change_Cache_Size_At_Runtime()
        {
            Parser.RegexCacheSize = 4;
            Assert.Equal(4, Parser.RegexCacheSize);
            Parser.TagRegexCache.Clear();
            Parser.ParseTemplate("Test 1 {{=<% %>=}}");
            Assert.Equal(2, Parser.TagRegexCache.Count);
            Parser.ParseTemplate("Test 2 {{={| |}=}}");
            Assert.Equal(3, Parser.TagRegexCache.Count);
            Parser.ParseTemplate("Test 3 {{=<: :>=}}");
            Assert.Equal(4, Parser.TagRegexCache.Count);
            Parser.ParseTemplate("Test 4 {{=|# #|=}}");
            Assert.Equal(4, Parser.TagRegexCache.Count);
            Parser.ParseTemplate("Test 5 {{=|# #|=}}");

            Assert.Equal(4, Parser.TagRegexCache.Count);

            Parser.RegexCacheSize = 2;
            Assert.Equal(2, Parser.RegexCacheSize);
            Assert.Equal(2, Parser.TagRegexCache.Count);
        }

        public static IEnumerable<object[]> TemplateParsingData()
        {
            return new[] 
            {
	            new { index = 1, name="", arguments = new List<ParserOutput> {}},
	            new { index = 2, name="{{hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 6,  },
	            }},
	            new { index = 3, name="{{hi.world}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "name", Value = "hi.world", Start = 0, End = 12,  },
	            }},
	            new { index = 4, name="{{hi . world}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "name", Value = "hi . world", Start = 0, End = 14,  },
	            }},
	            new { index = 5, name="{{ hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 7,  },
	            }},
	            new { index = 6, name="{{hi }}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 7,  },
	            }},
	            new { index = 7, name="{{ hi }}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 0, End = 8,  },
	            }},
	            new { index = 8, name="{{{hi}}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "&", Value = "hi", Start = 0, End = 8,  },
	            }},
	            new { index = 9, name="{{!hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "!", Value = "hi", Start = 0, End = 7,  },
	            }},
	            new { index = 10, name="{{! hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "!", Value = "hi", Start = 0, End = 8,  },
	            }},
	            new { index = 11, name="{{! hi }}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "!", Value = "hi", Start = 0, End = 9,  },
	            }},
	            new { index = 12, name="{{ !hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "!", Value = "hi", Start = 0, End = 8,  },
	            }},
	            new { index = 13, name="{{ ! hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "!", Value = "hi", Start = 0, End = 9,  },
	            }},
	            new { index = 14, name="{{ ! hi }}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "!", Value = "hi", Start = 0, End = 10,  },
	            }},
	            new { index = 15, name="a\n b", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n b", Start = 0, End = 4,  },
	            }},
	            new { index = 16, name="a{{hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a", Start = 0, End = 1,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 1, End = 7,  },
	            }},
	            new { index = 17, name="a {{hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a ", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 2, End = 8,  },
	            }},
	            new { index = 18, name=" a{{hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = " a", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 2, End = 8,  },
	            }},
	            new { index = 19, name=" a {{hi}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = " a ", Start = 0, End = 3,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 3, End = 9,  },
	            }},
	            new { index = 20, name="a{{hi}}b", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a", Start = 0, End = 1,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 1, End = 7,  },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 7, End = 8,  },
	            }},
	            new { index = 21, name="a{{hi}} b", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a", Start = 0, End = 1,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 1, End = 7,  },
		            new ParserOutput { TokenType = "text", Value = " b", Start = 7, End = 9,  },
	            }},
	            new { index = 22, name="a{{hi}}b ", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a", Start = 0, End = 1,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 1, End = 7,  },
		            new ParserOutput { TokenType = "text", Value = "b ", Start = 7, End = 9,  },
	            }},
	            new { index = 23, name="a\n{{hi}} b \n", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 2, End = 8,  },
		            new ParserOutput { TokenType = "text", Value = " b \n", Start = 8, End = 12,  },
	            }},
	            new { index = 24, name="a\n {{hi}} \nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n ", Start = 0, End = 3,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 3, End = 9,  },
		            new ParserOutput { TokenType = "text", Value = " \nb", Start = 9, End = 12,  },
	            }},
	            new { index = 25, name="a\n {{!hi}} \nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "!", Value = "hi", Start = 3, End = 10,  },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 12, End = 13,  },
	            }},
	            new { index = 26, name="a\n{{#a}}{{/a}}\nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 2, End = 8, ParentSectionEnd = 8, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 15, End = 16,  },
	            }},
	            new { index = 27, name="a\n {{#a}}{{/a}}\nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 9, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 16, End = 17,  },
	            }},
	            new { index = 28, name="a\n {{#a}}{{/a}} \nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 9, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 17, End = 18,  },
	            }},
	            new { index = 29, name="a\n{{#a}}\n{{/a}}\nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 2, End = 8, ParentSectionEnd = 9, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 16, End = 17,  },
	            }},
	            new { index = 30, name="a\n {{#a}}\n{{/a}}\nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 10, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 17, End = 18,  },
	            }},
	            new { index = 31, name="a\n {{#a}}\n{{/a}} \nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 10, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 18, End = 19,  },
	            }},
	            new { index = 32, name="a\n{{#a}}\n{{/a}}\n{{#b}}\n{{/b}}\nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 2, End = 8, ParentSectionEnd = 9, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "#", Value = "b", Start = 16, End = 22, ParentSectionEnd = 23, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 30, End = 31,  },
	            }},
	            new { index = 33, name="a\n {{#a}}\n{{/a}}\n{{#b}}\n{{/b}}\nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 10, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "#", Value = "b", Start = 17, End = 23, ParentSectionEnd = 24, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 31, End = 32,  },
	            }},
	            new { index = 34, name="a\n {{#a}}\n{{/a}}\n{{#b}}\n{{/b}} \nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ParentSectionEnd = 10, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "#", Value = "b", Start = 17, End = 23, ParentSectionEnd = 24, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 32, End = 33,  },
	            }},
	            new { index = 35, name="a\n{{#a}}\n{{#b}}\n{{/b}}\n{{/a}}\nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 2, End = 8, ChildTokens = new List<ParserOutput> {
			            new ParserOutput { TokenType = "#", Value = "b", Start = 9, End = 15, ParentSectionEnd = 16, ChildTokens = new List<ParserOutput>() },
		             }, ParentSectionEnd = 23, },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 30, End = 31,  },
	            }},
	            new { index = 36, name="a\n {{#a}}\n{{#b}}\n{{/b}}\n{{/a}}\nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ChildTokens = new List<ParserOutput> {
			            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16, ParentSectionEnd = 17, ChildTokens = new List<ParserOutput>() },
		             }, ParentSectionEnd = 24, },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 31, End = 32,  },
	            }},
	            new { index = 37, name="a\n {{#a}}\n{{#b}}\n{{/b}}\n{{/a}} \nb", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "text", Value = "a\n", Start = 0, End = 2,  },
		            new ParserOutput { TokenType = "#", Value = "a", Start = 3, End = 9, ChildTokens = new List<ParserOutput> {
			            new ParserOutput { TokenType = "#", Value = "b", Start = 10, End = 16, ParentSectionEnd = 17, ChildTokens = new List<ParserOutput>() },
		             }, ParentSectionEnd = 24, },
		            new ParserOutput { TokenType = "text", Value = "b", Start = 32, End = 33,  },
	            }},
	            new { index = 38, name="{{>abc}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = ">", Value = "abc", Start = 0, End = 8,  },
	            }},
	            new { index = 39, name="{{> abc }}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = ">", Value = "abc", Start = 0, End = 10,  },
	            }},
	            new { index = 40, name="{{ > abc }}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = ">", Value = "abc", Start = 0, End = 11,  },
	            }},
	            new { index = 41, name="{{=<% %>=}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "=", Value = "<% %>", Start = 0, End = 11,  },
	            }},
	            new { index = 42, name="{{= <% %> =}}", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "=", Value = "<% %>", Start = 0, End = 13,  },
	            }},
	            new { index = 43, name="{{=<% %>=}}<%={{ }}=%>", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "=", Value = "<% %>", Start = 0, End = 11,  },
		            new ParserOutput { TokenType = "=", Value = "{{ }}", Start = 11, End = 22,  },
	            }},
	            new { index = 44, name="{{=<% %>=}}<%hi%>", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "=", Value = "<% %>", Start = 0, End = 11,  },
		            new ParserOutput { TokenType = "name", Value = "hi", Start = 11, End = 17,  },
	            }},
	            new { index = 45, name="{{#a}}{{/a}}hi{{#b}}{{/b}}\n", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "#", Value = "a", Start = 0, End = 6, ParentSectionEnd = 6, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "hi", Start = 12, End = 14,  },
		            new ParserOutput { TokenType = "#", Value = "b", Start = 14, End = 20, ParentSectionEnd = 20, ChildTokens = new List<ParserOutput>() },
		            new ParserOutput { TokenType = "text", Value = "\n", Start = 26, End = 27,  },
	            }},
	            new { index = 46, name="{{a}}\n{{b}}\n\n{{#c}}\n{{/c}}\n", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "name", Value = "a", Start = 0, End = 5,  },
		            new ParserOutput { TokenType = "text", Value = "\n", Start = 5, End = 6,  },
		            new ParserOutput { TokenType = "name", Value = "b", Start = 6, End = 11,  },
		            new ParserOutput { TokenType = "text", Value = "\n\n", Start = 11, End = 13,  },
		            new ParserOutput { TokenType = "#", Value = "c", Start = 13, End = 19, ParentSectionEnd = 20, ChildTokens = new List<ParserOutput>() },
	            }},
	            new { index = 47, name="{{#foo}}\n  {{#a}}\n    {{b}}\n  {{/a}}\n{{/foo}}\n", arguments = new List<ParserOutput> {
		            new ParserOutput { TokenType = "#", Value = "foo", Start = 0, End = 8, ChildTokens = new List<ParserOutput> {
			            new ParserOutput { TokenType = "#", Value = "a", Start = 11, End = 17, ChildTokens = new List<ParserOutput> {
				            new ParserOutput { TokenType = "text", Value = "    ", Start = 18, End = 22,  },
				            new ParserOutput { TokenType = "name", Value = "b", Start = 22, End = 27,  },
				            new ParserOutput { TokenType = "text", Value = "\n", Start = 27, End = 28,  },
			             }, ParentSectionEnd = 30, },
		             }, ParentSectionEnd = 37, },
	            }},
            }.Select(x => new object[] { x.name, x.arguments });
        }
    }
}
