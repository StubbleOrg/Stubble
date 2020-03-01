using Stubble.Core.Builders;
using Stubble.Core.Exceptions;
using Stubble.Core.Loaders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Stubble.Core.Tests
{
    public class AsyncStubbleTest
    {
        [Fact]
        public async Task It_Can_Render_The_Template_Async()
        {
            var stubble = new StubbleBuilder()
                .Build();

            var output = await stubble.RenderAsync("{{Foo}}", new { Foo = "Bar" });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public async Task It_Can_Render_WithPartials_FromLoader_Async()
        {
            var stubble = new StubbleBuilder()
                .Configure(builder => {
                    builder.SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string>
                    {
                        { "foo", "{{Foo}} this" }
                    }));
                }).Build();

            var output = await stubble.RenderAsync("{{> foo}}", new { Foo = "Bar" });
            Assert.Equal("Bar this", output);
        }

        [Fact]
        public async Task It_Can_Render_WithPartials_FromArgs_Async()
        {
            var stubble = new StubbleBuilder().Build();

            var output = await stubble.RenderAsync("{{> foo}}", new { Foo = "Bar" }, new Dictionary<string, string>
                {
                    { "foo", "{{Foo}} this" }
                });
            Assert.Equal("Bar this", output);
        }

        [Fact]
        public async Task It_Can_Render_Literals_Async()
        {
            var stubble = new StubbleBuilder().Build();

            var output = await stubble.RenderAsync("Literal test", null);
            Assert.Equal("Literal test", output);
        }

        [Fact]
        public async Task It_Can_Render_Inverted_Sections_Async()
        {
            var stubble = new StubbleBuilder().Build();

            var output = await stubble.RenderAsync(@"{{^True}}Not True{{/True}}", new { True = false });
            Assert.Equal("Not True", output);
        }

        [Fact]
        public async Task It_Should_Error_After_N_Recursions_Async()
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

            var stubble = new StubbleVisitorRenderer();
            var ex =
                await Assert.ThrowsAsync<StubbleException>(async () => await stubble.RenderAsync(rowTemplate, treeData, new Dictionary<string, string>
                {
                    { "row", rowTemplate },
                    { "column", columnTemplate },
                    { "text", textTemplate }
                }));

            Assert.Equal("You have reached the maximum recursion limit of 256.", ex.Message);
        }

        [Fact]
        public async Task It_Can_Render_With_LambdaSection_NoDynamic_Async()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = await stubble.RenderAsync("{{#Foo}}Foo{{/Foo}}", new { Foo = new Func<string, object>((str) => str + " Bar") });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public async Task It_Can_Render_Enumerators_Async()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = await stubble.RenderAsync("{{#Items}}{{.}}{{/Items}}", new { Items = "abcdefg".ToCharArray().GetEnumerator() });
            Assert.Equal("abcdefg", output);
        }

        [Fact]
        public async Task It_Can_Render_With_LambdaToken_NoDynamic_Async()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = await stubble.RenderAsync("{{Foo}}", new { Foo = new Func<object>(() => "Bar") });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public async Task It_Can_Render_With_LambdaToken_Interpolation_NoDynamic_Async()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = await stubble.RenderAsync("{{Foo}}", new { Foo = new Func<object>(() => "{{Bar}}"), Bar = "FooBar" });
            Assert.Equal("FooBar", output);
        }

        [Fact]
        public async Task It_Can_Render_With_SectionLambda_WithContext_Async()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = await stubble.RenderAsync("{{#Foo}}{{/Foo}}", new { Foo = new Func<dynamic, string, object>((ctx, str) => ctx.Bar), Bar = "FooBar" });
            Assert.Equal("FooBar", output);
        }

        [Fact]
        public async Task It_Should_Allow_An_NonAsync_Render_Function_WithContext_In_Lambda()
        {
            var stubble = new StubbleBuilder()
                .Build();

            var obj = new
            {
                Value = "a",
                ValueDictionary = new Dictionary<string, string>
                {
                    { "a", "A is Cool" },
                    { "b", "B is Cool" },
                },
                ValueRender = new Func<dynamic, string, Func<string, string>, object>((dynamic ctx, string text, Func<string, string> render)
                    => "{{ValueDictionary." + render(ctx.Value) + "}}")
            };

            var result = await stubble.RenderAsync("{{#ValueRender}}{{/ValueRender}}", obj);
            Assert.Equal("A is Cool", result);
        }

        [Fact]
        public async Task It_Should_Allow_An_NonAsync_Render_Function_In_Lambda()
        {
            var stubble = new StubbleBuilder()
                .Build();

            var obj = new
            {
                Value = "a",
                ValueDictionary = new Dictionary<string, string>
                {
                    { "a", "A is Cool" },
                    { "b", "B is Cool" },
                },
                ValueRender = new Func<string, Func<string, string>, object>((string text, Func<string, string> render)
                    => "{{ValueDictionary." + render("{{Value}}") + "}}")
            };

            var result = await stubble.RenderAsync("{{#ValueRender}}{{/ValueRender}}", obj);
            Assert.Equal("A is Cool", result);
        }

        [Fact]
        public async Task It_Should_Allow_Async_Lambda_Method_Without_Context_In_Async_Renderer()
        {
            var stubble = new StubbleVisitorRenderer();

            var output = await stubble.RenderAsync("{{#TestMethodAsync}}Hello World!{{/TestMethodAsync}}", new {
                TestMethodAsync = new Func<string, Task<object>>(async str =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    return str.ToLower();
                }),
            });

            Assert.Equal("hello world!", output);
        }

        [Fact]
        public async Task It_Should_Allow_Async_Lambda_Method_With_Context_In_Async_Renderer()
        {
            var stubble = new StubbleVisitorRenderer();

            var output = await stubble.RenderAsync("{{#TestMethodAsync}}Hello World!{{/TestMethodAsync}}", new
            {
                TestMethodAsync = new Func<dynamic, string, Task<object>>(async (ctx, str) =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    return str.ToLower();
                }),
            });

            Assert.Equal("hello world!", output);
        }

        [Fact]
        public async Task It_Should_Allow_Async_Lambda_Method_WithRenderFunction_Without_Context_In_Async_Renderer()
        {
            var stubble = new StubbleVisitorRenderer();

            var output = await stubble.RenderAsync("{{#TestMethodAsync}}Hello World!{{/TestMethodAsync}}", new
            {
                Foo = "Foo",
                TestMethodAsync = new Func<string, Func<string, Task<string>>, Task<object>>(async (str, renderFunc) =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    return (await renderFunc(str)).ToLower();
                }),
            });

            Assert.Equal("hello world!", output);
        }

        [Fact]
        public async Task It_Should_Allow_Async_Lambda_Method_WithRenderFunction_With_Context_In_Async_Renderer()
        {
            var stubble = new StubbleVisitorRenderer();

            var output = await stubble.RenderAsync("{{#TestMethodAsync}}Hello World!{{/TestMethodAsync}}", new
            {
                TestMethodAsync = new Func<dynamic, string, Func<string, Task<string>>, Task<object>>(async (ctx, str, renderFunc) =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    return (await renderFunc(str)).ToLower();
                }),
            });

            Assert.Equal("hello world!", output);
        }
    }
}
