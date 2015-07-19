using System;
using System.Collections.Generic;
using Stubble.Core.Classes;
using Xunit;

namespace Stubble.Core.Tests
{
    public class StubbleTest
    {
        [Fact]
        public void It_Can_Clear_The_Cache()
        {
            var stubble = new Stubble();
            stubble.CacheTemplate("Test {{Foo}} Test");
            Assert.Equal(1, stubble.Writer.Cache.Count);
            stubble.ClearCache();
            Assert.Equal(0, stubble.Writer.Cache.Count);
        }

        [Fact]
        public void It_Can_Pass_Parse_Arguments()
        {
            var stubble = new Stubble();
            var result1 = stubble.Parse("Test {{Foo}} Test 1");
            var result2 = stubble.Parse("Test {{Foo}} Test 2", "{{ }}");
            var result3 = stubble.Parse("Test {{Foo}} Test 3", new Tags("{{", "}}"));
            Assert.NotEmpty(result1);
            Assert.NotEmpty(result2);
            Assert.NotEmpty(result3);
        }

        [Fact]
        public void It_Can_Cache_Templates()
        {
            var stubble = new Stubble();
            stubble.CacheTemplate("Test {{Foo}} Test 1");
            Assert.Equal(1, stubble.Writer.Cache.Count);
            stubble.CacheTemplate("Test {{Foo}} Test 2", "{{ }}");
            Assert.Equal(2, stubble.Writer.Cache.Count);
            stubble.CacheTemplate("Test {{Foo}} Test 3", new Tags("{{", "}}"));
            Assert.Equal(3, stubble.Writer.Cache.Count);
        }

        [Fact]
        public void It_Can_Render_WithoutPartials()
        {
            var stubble = new Stubble();
            var output = stubble.Render("{{Foo}}", new { Foo = "Bar" });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_WithPartials()
        {
            var stubble = new Stubble();
            var output = stubble.Render("{{> inner}}", new { Foo = "Bar" }, new Dictionary<string, string> { { "inner", "{{Foo}}" } });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_WithPartials_FromLoader()
        {
            var stubble =new StubbleBuilder()
                .SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string>
                {
                    {"foo", "{{Foo}} this"}
                })).Build();

            var output = stubble.Render("{{> foo}}", new { Foo = "Bar" });
            Assert.Equal("Bar this", output);
        }

        [Fact]
        public void It_Should_Not_Render_If_Partial_Doesnt_Exist_In_Loader()
        {
            var stubble = new StubbleBuilder()
                  .SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string>
                {
                    {"foo", "{{Foo}} this"}
                })).Build();

            var output = stubble.Render("{{> foo2}}", new { Foo = "Bar" });
            Assert.Equal("", output);
        }

        [Fact]
        public void It_Doesnt_Error_When_Partial_Is_Used_But_None_Are_Given()
        {
            var stubble = new Stubble();
            var output = stubble.Render("{{> inner}}", new { Foo = "Bar" });
            Assert.Equal("", output);
        }

        [Fact]
        public void It_Can_Render_WithoutData()
        {
            var stubble = new Stubble();
            var output = stubble.Render("I Have No Data :(", null);
            Assert.Equal("I Have No Data :(", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_NoDynamic()
        {
            var stubble = new Stubble();
            var output = stubble.Render("{{Foo}}", new { Foo = new Func<object>(() => "Bar") });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_Dynamic()
        {
            var stubble = new Stubble();
            var output = stubble.Render("{{Foo}}", new { BarValue = "Bar", Foo = new Func<dynamic, object>((context) => context.BarValue) });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_NoDynamic()
        {
            var stubble = new Stubble();
            var output = stubble.Render("{{#Foo}}Foo{{/Foo}}", new { Foo = new Func<string, object>((str) => str + " Bar") });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_Dynamic()
        {
            var stubble = new Stubble();
            var output = stubble.Render("{{#Foo}}Foo{{/Foo}}", new
            {
                BarValue = "Bar", Foo = new Func<dynamic, string, object>((context, str) => str + " " + context.BarValue)
            });
            Assert.Equal("Foo Bar", output);
        }
    }
}
