using System;
using System.Collections.Generic;
using Stubble.Core.Classes;
using Xunit;

namespace Stubble.Core.Tests
{
    public class StubbleStaticTest
    {
        [Fact]
        public void It_Can_Clear_The_Cache()
        {
            StubbleStatic.ClearCache();
            StubbleStatic.CacheTemplate("Test {{Foo}} Test");
            Assert.Equal(1, StubbleStatic.Instance.Writer.Cache.Count);
            StubbleStatic.ClearCache();
            Assert.Equal(0, StubbleStatic.Instance.Writer.Cache.Count);
        }

        [Fact]
        public void It_Can_Pass_Parse_Arguments()
        {
            var result1 = StubbleStatic.Parse("Test {{Foo}} Test 1");
            var result2 = StubbleStatic.Parse("Test {{Foo}} Test 2", "{{ }}");
            var result3 = StubbleStatic.Parse("Test {{Foo}} Test 3", new Tags("{{", "}}"));
            Assert.NotEmpty(result1);
            Assert.NotEmpty(result2);
            Assert.NotEmpty(result3);
        }

        [Fact]
        public void It_Can_Cache_Templates()
        {
            StubbleStatic.ClearCache();
            StubbleStatic.CacheTemplate("Test {{Foo}} Test 1");
            Assert.Equal(1, StubbleStatic.Instance.Writer.Cache.Count);
            StubbleStatic.CacheTemplate("Test {{Foo}} Test 2", "{{ }}");
            Assert.Equal(2, StubbleStatic.Instance.Writer.Cache.Count);
            StubbleStatic.CacheTemplate("Test {{Foo}} Test 3", new Tags("{{", "}}"));
            Assert.Equal(3, StubbleStatic.Instance.Writer.Cache.Count);
        }

        [Fact]
        public void It_Can_Render_WithoutPartials()
        {
            var output = StubbleStatic.Render("{{Foo}}", new { Foo = "Bar" });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_WithPartials()
        {
            var output = StubbleStatic.Render("{{> inner}}", new { Foo = "Bar" }, new Dictionary<string, string> { { "inner", "{{Foo}}" } });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Doesnt_Error_When_Partial_Is_Used_But_None_Are_Given()
        {
            var output = StubbleStatic.Render("{{> inner}}", new { Foo = "Bar" });
            Assert.Equal("", output);
        }

        [Fact]
        public void It_Can_Render_WithoutData()
        {
            var output = StubbleStatic.Render("I Have No Data :(", null);
            Assert.Equal("I Have No Data :(", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_NoDynamic()
        {
            var output = StubbleStatic.Render("{{Foo}}", new { Foo = new Func<object>(() => "Bar") });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_Dynamic()
        {
            var output = StubbleStatic.Render("{{Foo}}", new { BarValue = "Bar", Foo = new Func<dynamic, object>((context) => context.BarValue) });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_NoDynamic()
        {
            var output = StubbleStatic.Render("{{#Foo}}Foo{{/Foo}}", new { Foo = new Func<string, object>((str) => str + " Bar") });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_Dynamic()
        {
            var output = StubbleStatic.Render("{{#Foo}}Foo{{/Foo}}", new
            {
                BarValue = "Bar",
                Foo = new Func<dynamic, string, object>((context, str) => str + " " + context.BarValue)
            });
            Assert.Equal("Foo Bar", output);
        }
    }
}
