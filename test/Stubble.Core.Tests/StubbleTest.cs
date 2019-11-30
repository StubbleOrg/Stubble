// <copyright file="StubbleTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stubble.Core.Builders;
using Stubble.Core.Classes;
using Stubble.Core.Exceptions;
using Stubble.Core.Loaders;
using Stubble.Core.Parser;
using Stubble.Core.Settings;
using Xunit;

namespace Stubble.Core.Tests
{
    public class StubbleTest
    {
        [Fact]
        public void It_Can_Pass_Parse_Arguments()
        {
            var result1 = MustacheParser.Parse("Test {{Foo}} Test 1");
            var result2 = MustacheParser.Parse("Test {{Foo}} Test 3", new Tags("{{", "}}"));
            Assert.NotEmpty(result1.Children);
            Assert.NotEmpty(result2.Children);
        }

        [Fact]
        public void It_Can_Cache_Templates()
        {
            var parser = new CachedMustacheParser(15);

            var stubble = new StubbleBuilder()

                .Configure(b => b.SetMustacheParser(parser))
                .Build();

            var stubbleTags = new StubbleBuilder()

                .Configure(b =>
                {
                    b.SetDefaultTags(new Tags("%[", "]%"))
                     .SetMustacheParser(parser);
                })
                .Build();

            stubble.Render("Test {{Foo}} Test 1", null);

            var cachingParser = stubble.RendererSettings.Parser as CachedMustacheParser;
            Assert.NotNull(cachingParser);

            Assert.Single(cachingParser.Cache);
            stubbleTags.Render("Test %[Foo]% Test 3", null);
            Assert.Equal(2, cachingParser.Cache.Count);
        }

        [Fact]
        public void It_Can_Render_WithoutPartials()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = stubble.Render("{{Foo}}", new { Foo = "Bar" });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_WithPartials()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = stubble.Render("{{> inner}}", new { Foo = "Bar" }, new Dictionary<string, string> { { "inner", "{{Foo}}" } });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_WithPartials_FromLoader()
        {
            var stubble = new StubbleBuilder()

                .Configure(b => b.SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string>
                {
                    { "foo", "{{Foo}} this" }
                })))
                .Build();

            var output = stubble.Render("{{> foo}}", new { Foo = "Bar" });
            Assert.Equal("Bar this", output);
        }

        [Fact]
        public void It_Should_Not_Render_If_Partial_Doesnt_Exist_In_Loader()
        {
            var stubble = new StubbleBuilder()

                .Configure(b => b.SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string>
                {
                    { "foo", "{{Foo}} this" }
                })))
                .Build();

            var output = stubble.Render("{{> foo2}}", new { Foo = "Bar" });
            Assert.Equal("", output);
        }

        [Fact]
        public void It_Doesnt_Error_When_Partial_Is_Used_But_None_Are_Given()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = stubble.Render("{{> inner}}", new { Foo = "Bar" });
            Assert.Equal("", output);
        }

        [Fact]
        public void It_Can_Render_WithoutData()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = stubble.Render("I Have No Data :(", null);
            Assert.Equal("I Have No Data :(", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_NoDynamic()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = stubble.Render("{{Foo}}", new { Foo = new Func<object>(() => "Bar") });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaToken_Dynamic()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = stubble.Render("{{Foo}}", new { BarValue = "Bar", Foo = new Func<dynamic, object>((context) => context.BarValue) });
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_NoDynamic()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = stubble.Render("{{#Foo}}Foo{{/Foo}}", new { Foo = new Func<string, object>((str) => str + " Bar") });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Can_Render_With_LambdaSection_Dynamic()
        {
            var stubble = new StubbleVisitorRenderer();
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

            var stubble = new StubbleVisitorRenderer();
            var ex =
                Assert.Throws<StubbleException>(() => stubble.Render(rowTemplate, treeData, new Dictionary<string, string>
                {
                    { "row", rowTemplate },
                    { "column", columnTemplate },
                    { "text", textTemplate }
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

            var stubble = new StubbleBuilder().Configure(b => b.SetMaxRecursionDepth(128)).Build();
            var ex =
                Assert.Throws<StubbleException>(() => stubble.Render(rowTemplate, treeData, new Dictionary<string, string>
                {
                    { "row", rowTemplate },
                    { "column", columnTemplate },
                    { "text", textTemplate }
                }));

            Assert.Equal("You have reached the maximum recursion limit of 128.", ex.Message);
        }

        [Fact]
        public void It_Should_Be_Able_To_Skip_Recursive_Lookups()
        {
            var settings =
            new RenderSettings
            {
                SkipRecursiveLookup = true
            };
            var stubble = new StubbleVisitorRenderer();
            var output = stubble.Render(
                "{{FooValue}} {{#Foo}}{{FooValue}}{{BarValue}}{{/Foo}}",
                new
                {
                    FooValue = "Foo",
                    Foo = new
                    {
                        BarValue = "Bar"
                    }
                },
                settings);

            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Should_Be_Able_To_Take_Partials_And_Render_Settings()
        {
            var stubble = new StubbleVisitorRenderer();
            var output = stubble.Render(
                "{{FooValue}} {{#Foo}}{{> FooBar}}{{/Foo}}",
                new
                {
                    FooValue = "Foo",
                    Foo = new
                    {
                        BarValue = "Bar"
                    }
                },
                new Dictionary<string, string>
                {
                    { "FooBar", "{{FooValue}}{{BarValue}}" }
                },
                new RenderSettings { SkipRecursiveLookup = true });
            Assert.Equal("Foo Bar", output);
        }

        [Fact]
        public void It_Should_Have_Fresh_Depth_On_Each_Render()
        {
            var stubble = new StubbleBuilder().Configure(b => b.SetMaxRecursionDepth(128)).Build();

            for (var i = 0; i < 256; i++)
            {
                Assert.NotNull(stubble.Render("{{Foo}}", new { Foo = 1 }));
            }
        }

        [Fact]
        public void It_Should_Handle_Dictionaries_With_Empty_Keys()
        {
            var stubble = new StubbleBuilder().Build();

            Dictionary<string, object> dataHash = new Dictionary<string, object>();
            Assert.Equal("", stubble.Render("{{Foo.Value}}", dataHash));
        }

        [Fact]
        public void It_Should_Skip_Html_Encoding_With_Setting()
        {
            var stubble = new StubbleBuilder()
                .Build();

            var obj = new
            {
                Html = "<b>Html</b>"
            };

            var result = stubble.Render("{{Html}}\n{{{Html}}}", obj, new RenderSettings
            {
                SkipHtmlEncoding = true
            });
            Assert.Equal("<b>Html</b>\n<b>Html</b>", result);
        }

        [Fact]
        public async Task It_Should_Skip_Html_Encoding_With_Setting_Async()
        {
            var stubble = new StubbleBuilder()
                .Build();

            var obj = new
            {
                Html = "<b>Html</b>"
            };

            var result = await stubble.RenderAsync("{{Html}}\n{{{Html}}}", obj, new RenderSettings
            {
                SkipHtmlEncoding = true
            });
            Assert.Equal("<b>Html</b>\n<b>Html</b>", result);
        }

        [Fact]
        public void It_Should_Loop_Dictionary_When_Allowed()
        {
            var stubble = new StubbleBuilder()
                .Configure(conf =>
                {
                    conf.SetSectionBlacklistTypes(new HashSet<Type>
                    {
                        typeof(string)
                    });
                })
                .Build();

            var obj = new
            {
                Dict = new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { "key2", "value2" },
                    { "key3", "value3" },
                }
            };

            var result = stubble.Render("{{#Dict}}{{Key}}|{{Value}}.{{/Dict}}", obj);

            Assert.Equal("key1|value1.key2|value2.key3|value3.", result);
        }

        [Fact]
        public async Task It_Should_Indent_Partials_When_Folding_Literals_Correctly()
        {
            var tpl = @"<div>
    {{> Body}}
</div>";
            var partial = @"<a href=""{{Url}}"">
    My Link
</a>";

            var stubble = new StubbleBuilder()
                .Configure(conf =>
                {
                    conf.SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string>
                    {
                        { "Body", partial }
                    }));
                })
                .Build();

            var obj = new
            {
                Url = "MyUrl"
            };

            var result = stubble.Render(tpl, obj);

            Assert.Equal(@"<div>
    <a href=""MyUrl"">
        My Link
    </a></div>", result);

            var resultAsync = await stubble.RenderAsync(tpl, obj);

            Assert.Equal(@"<div>
    <a href=""MyUrl"">
        My Link
    </a></div>", resultAsync);
        }

        [Fact]
        public void It_Should_Allow_A_Render_Function_In_Lambda()
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
                    => "{{ValueDictionary." + render("{{Value}}") +"}}")
            };

            var result = stubble.Render("{{#ValueRender}}{{/ValueRender}}", obj);
            Assert.Equal("A is Cool", result);
        }

        [Fact]
        public void It_Should_Allow_A_Render_Function_WithContext_In_Lambda()
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

            var result = stubble.Render("{{#ValueRender}}{{/ValueRender}}", obj);
            Assert.Equal("A is Cool", result);
        }

        [Fact]
        public void It_Should_Throw_When_Ambiguous_Match()
        {
            var stubble = new StubbleBuilder()
                .Configure(settings =>
                {
                    settings.SetIgnoreCaseOnKeyLookup(true);
                })
                .Build();

            var ex = Assert.Throws<StubbleAmbigousMatchException>(() => stubble.Render("{{name}}", new { Name = "foo", name = "bar" }));

            Assert.Equal("Ambiguous match found when looking up key: 'name'", ex.Message);
        }
    }
}
