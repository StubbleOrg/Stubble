using System;
using System.Collections.Generic;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Exceptions;
using Xunit;

namespace Stubble.Core.Tests
{
    public class StubbleTest
    {
        [Fact]
        public void It_Can_Clear_The_Cache()
        {
            var stubble = new StubbleRenderer();
            stubble.CacheTemplate("Test {{Foo}} Test");
            Assert.Equal(1, stubble.Writer.Cache.Count);
            stubble.ClearCache();
            Assert.Equal(0, stubble.Writer.Cache.Count);
        }

        [Fact]
        public void It_Can_Pass_Parse_Arguments()
        {
            var stubble = new StubbleRenderer();
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
            var stubble = new StubbleRenderer();
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
            var stubble = new StubbleRenderer();
            var output = stubble.Render("{{Foo}}", new { Foo = "Bar" });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_WithPartials()
        {
            var stubble = new StubbleRenderer();
            var output = stubble.Render("{{> inner}}", new { Foo = "Bar" }, new Dictionary<string, string> { { "inner", "{{Foo}}" } });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_WithPartials_FromLoader()
        {
            var stubble = new StubbleBuilder()
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
            var stubble = new StubbleRenderer();
            var output = stubble.Render("{{> inner}}", new { Foo = "Bar" });
            Assert.Equal("", output);
        }

        [Fact]
        public void It_Can_Render_WithoutData()
        {
            var stubble = new StubbleRenderer();
            var output = stubble.Render("I Have No Data :(", null);
            Assert.Equal("I Have No Data :(", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_NoDynamic()
        {
            var stubble = new StubbleRenderer();
            var output = stubble.Render("{{Foo}}", new { Foo = new Func<object>(() => "Bar") });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_Dynamic()
        {
            var stubble = new StubbleRenderer();
            var output = stubble.Render("{{Foo}}", new { BarValue = "Bar", Foo = new Func<dynamic, object>((context) => context.BarValue) });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_NoDynamic()
        {
            var stubble = new StubbleRenderer();
            var output = stubble.Render("{{#Foo}}Foo{{/Foo}}", new { Foo = new Func<string, object>((str) => str + " Bar") });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_Dynamic()
        {
            var stubble = new StubbleRenderer();
            var output = stubble.Render("{{#Foo}}Foo{{/Foo}}", new
            {
                BarValue = "Bar",
                Foo = new Func<dynamic, string, object>((context, str) => str + " " + context.BarValue)
            });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Should_Error_After_N_Recursions()
        {
            const string rowTemplate = @"
            <div class='row'>
                {{#content}}
                    {{#is_column}}
                        {{>column}}
                    {{/is_column}}
                {{/content}}
            </div>";

            const string columnTemplate = @"
            <div class='column'>
                {{#content}}
                    {{#is_text}}
                        {{>text}}
                    {{/is_text}}
                    {{#is_row}}
                        {{>row}}
                    {{/is_row}}
                {{/content}}
            </div>";

            const string textTemplate = @"
            <span class='text'>
                {{text}}
            </span>";

            var treeData = new
            {
                is_row = true,
                content = new
                {
                    is_column = true,
                    content = new[]
                    {
                        new
                        {
                            is_text = true,
                            text = "Hello World!"
                        }
                    }
                }
            };

            var stubble = new StubbleRenderer();
            var ex =
                Assert.Throws<StubbleException>(() => stubble.Render(rowTemplate, treeData, new Dictionary<string, string>
                {
                    {"row", rowTemplate},
                    {"column", columnTemplate},
                    {"text", textTemplate}
                }));

            Assert.Equal("You have reached the maximum recursion limit of 256.", ex.Message);
        }

        [Fact]
        public void It_Should_Be_Able_To_Change_Max_Recursion_Depth()
        {
            const string rowTemplate = @"
            <div class='row'>
                {{#content}}
                    {{#is_column}}
                        {{>column}}
                    {{/is_column}}
                {{/content}}
            </div>";

            const string columnTemplate = @"
            <div class='column'>
                {{#content}}
                    {{#is_text}}
                        {{>text}}
                    {{/is_text}}
                    {{#is_row}}
                        {{>row}}
                    {{/is_row}}
                {{/content}}
            </div>";

            const string textTemplate = @"
            <span class='text'>
                {{text}}
            </span>";

            var treeData = new
            {
                is_row = true,
                content = new
                {
                    is_column = true,
                    content = new[]
                    {
                        new
                        {
                            is_text = true,
                            text = "Hello World!"
                        }
                    }
                }
            };

            var stubble = new StubbleBuilder().SetMaxRecursionDepth(128).Build();
            var ex =
                Assert.Throws<StubbleException>(() => stubble.Render(rowTemplate, treeData, new Dictionary<string, string>
                {
                    {"row", rowTemplate},
                    {"column", columnTemplate},
                    {"text", textTemplate}
                }));

            Assert.Equal("You have reached the maximum recursion limit of 128.", ex.Message);
        }

        [Fact]
        public void It_Should_Be_Able_To_Skip_Recursive_Lookups()
        {
            var stubble = new StubbleRenderer();
            var output = stubble.Render("{{FooValue}} {{#Foo}}{{FooValue}}{{BarValue}}{{/Foo}}", new
            {
                FooValue = "Foo",
                Foo = new {
                    BarValue = "Bar"
                }
            }, new RenderSettings { SkipRecursiveLookup = true });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Should_Be_Able_To_Take_Partials_And_Render_Settings()
        {
            var stubble = new StubbleRenderer();
            var output = stubble.Render("{{FooValue}} {{#Foo}}{{> FooBar}}{{/Foo}}", new
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
        public void It_Should_Have_Fresh_Depth_On_Each_Render()
        {
            var stubble = new StubbleBuilder().SetMaxRecursionDepth(128).Build();

            for (var i = 0; i < 256; i++)
            {
                Assert.NotNull(stubble.Render("{{Foo}}", new { Foo = 1 }));
            }
        }
    }
}
