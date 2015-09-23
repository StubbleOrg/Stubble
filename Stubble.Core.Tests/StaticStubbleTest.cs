using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stubble.Core.Classes;
using Stubble.Core.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Stubble.Core.Tests
{
    public class StaticStubbleTest
    {
        private readonly ITestOutputHelper _output;
        public StaticStubbleTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void It_Can_Clear_The_Cache()
        {
            StaticStubbleRenderer.ClearCache();
            StaticStubbleRenderer.CacheTemplate("Test {{Foo}} Test");
            Assert.Equal(1, StaticStubbleRenderer.Instance.Writer.Cache.Count);
            StaticStubbleRenderer.ClearCache();
            Assert.Equal(0, StaticStubbleRenderer.Instance.Writer.Cache.Count);
        }

        [Fact]
        public void It_Can_Pass_Parse_Arguments()
        {
            var result1 = StaticStubbleRenderer.Parse("Test {{Foo}} Test 1");
            var result2 = StaticStubbleRenderer.Parse("Test {{Foo}} Test 2", "{{ }}");
            var result3 = StaticStubbleRenderer.Parse("Test {{Foo}} Test 3", new Tags("{{", "}}"));
            Assert.NotEmpty(result1);
            Assert.NotEmpty(result2);
            Assert.NotEmpty(result3);
        }

        [Fact]
        public void It_Can_Cache_Templates()
        {
            StaticStubbleRenderer.ClearCache();
            StaticStubbleRenderer.CacheTemplate("Test {{Foo}} Test 1");
            Assert.Equal(1, StaticStubbleRenderer.Instance.Writer.Cache.Count);
            StaticStubbleRenderer.CacheTemplate("Test {{Foo}} Test 2", "{{ }}");
            Assert.Equal(2, StaticStubbleRenderer.Instance.Writer.Cache.Count);
            StaticStubbleRenderer.CacheTemplate("Test {{Foo}} Test 3", new Tags("{{", "}}"));
            Assert.Equal(3, StaticStubbleRenderer.Instance.Writer.Cache.Count);
        }

        [Fact]
        public void It_Can_Render_WithoutPartials()
        {
            var output = StaticStubbleRenderer.Render("{{Foo}}", new { Foo = "Bar" });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_WithPartials()
        {
            var output = StaticStubbleRenderer.Render("{{> inner}}", new { Foo = "Bar" }, new Dictionary<string, string> { { "inner", "{{Foo}}" } });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Doesnt_Error_When_Partial_Is_Used_But_None_Are_Given()
        {
            var output = StaticStubbleRenderer.Render("{{> inner}}", new { Foo = "Bar" });
            Assert.Equal("", output);
        }

        [Fact]
        public void It_Can_Render_WithoutData()
        {
            var output = StaticStubbleRenderer.Render("I Have No Data :(", null);
            Assert.Equal("I Have No Data :(", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_NoDynamic()
        {
            var output = StaticStubbleRenderer.Render("{{Foo}}", new { Foo = new Func<object>(() => "Bar") });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_Dynamic()
        {
            var output = StaticStubbleRenderer.Render("{{Foo}}", new { BarValue = "Bar", Foo = new Func<dynamic, object>((context) => context.BarValue) });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_NoDynamic()
        {
            var output = StaticStubbleRenderer.Render("{{#Foo}}Foo{{/Foo}}", new { Foo = new Func<string, object>((str) => str + " Bar") });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_Dynamic()
        {
            var output = StaticStubbleRenderer.Render("{{#Foo}}Foo{{/Foo}}", new
            {
                BarValue = "Bar",
                Foo = new Func<dynamic, string, object>((context, str) => str + " " + context.BarValue)
            });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Should_Be_Able_To_Skip_Recursive_Lookups()
        {
            var output = StaticStubbleRenderer.Render("{{FooValue}} {{#Foo}}{{FooValue}}{{BarValue}}{{/Foo}}", new
            {
                FooValue = "Foo",
                Foo = new
                {
                    BarValue = "Bar"
                }
            }, new RenderSettings { SkipRecursiveLookup = true });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Should_Be_Able_To_Take_Partials_And_Render_Settings()
        {
            var output = StaticStubbleRenderer.Render("{{FooValue}} {{#Foo}}{{> FooBar}}{{/Foo}}", new
            {
                FooValue = "Foo",
                Foo = new
                {
                    BarValue = "Bar"
                }
            }, new Dictionary<string, string>
            {
                { "FooBar", "{{FooValue}}{{BarValue}}" }
            }, new RenderSettings { SkipRecursiveLookup = true });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Should_Map_To_The_IStubbleRenderer_Interface()
        {
            var staticMethods =
                typeof (StaticStubbleRenderer).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => !x.IsSpecialName).ToList();
            var interfaceMethods =
                typeof (IStubbleRenderer).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => !x.IsSpecialName).ToList();

            foreach (var method in interfaceMethods)
            {
                var methodParams = method.GetParameters();
                var item = staticMethods.FirstOrDefault(x =>
                {
                    var staticParams = x.GetParameters();
                    var allParamsMatch = staticParams
                        .Any(z => methodParams.Any(y => y.ParameterType == z.ParameterType && y.Name == z.Name));

                    return x.Name.Equals(method.Name) &&
                           method.ReturnType.IsAssignableFrom(x.ReturnType) &&
                           (!staticParams.Any() || allParamsMatch);

                });
                _output.WriteLine(method.ToString());
                Assert.NotNull(item);
            }
        }
    }
}
