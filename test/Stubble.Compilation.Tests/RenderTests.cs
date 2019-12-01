using Stubble.Compilation.Builders;
using Stubble.Compilation.Settings;
using Stubble.Core.Exceptions;
using Stubble.Test.Shared.Spec;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Stubble.Core.Loaders;
using Xunit;
using System.Linq.Expressions;

namespace Stubble.Compilation.Tests
{
    public class RenderTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void CompilationRenderer_SpecialTests(SpecTest data)
        {
            var builder = new CompilerSettingsBuilder();

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            if (data.ExpectedException != null)
            {
                var ex = Assert.Throws(data.ExpectedException.GetType(), () =>
                {
                    CompileAndRender();
                });

                Assert.Equal(data.ExpectedException.Message, ex.Message);
            }
            else
            {
                Assert.Equal(data.Expected, CompileAndRender());
            }

            string CompileAndRender()
            {
                var output = data.Partials != null ?
                    stubble.Compile(data.Template, data.Data, data.Partials) :
                    stubble.Compile(data.Template, data.Data);

                return (string)output(data.Data);
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task CompilationRenderer_SpecialTests_Async(SpecTest data)
        {
            var builder = new CompilerSettingsBuilder();

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            if (data.ExpectedException != null)
            {
                var ex = await Assert.ThrowsAsync(data.ExpectedException.GetType(), CompileAndRender);

                Assert.Equal(data.ExpectedException.Message, ex.Message);
            }
            else
            {
                Assert.Equal(data.Expected, await CompileAndRender());
            }

            async Task<string> CompileAndRender()
            {
                var output = data.Partials != null ?
                    await stubble.CompileAsync(data.Template, data.Data, data.Partials) :
                    await stubble.CompileAsync(data.Template, data.Data);

                return (string)output(data.Data);
            }
        }

        [Fact]
        public void CompilationRenderer_IgnoreCaseShouldIgnoreCase()
        {
            var builder = new CompilerSettingsBuilder()
                .SetIgnoreCaseOnKeyLookup(true);

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var arg = new
            {
                Foo = "Bar"
            };

            var ignoreCase = stubble.Compile("{{foo}}", arg);

            Assert.Equal("Bar", ignoreCase(arg));
        }

        [Fact]
        public void CompilationRenderer_ShouldBeCaseSensitiveByDefault()
        {
            var builder = new CompilerSettingsBuilder();

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var arg = new
            {
                Foo = "Bar"
            };

            var func = stubble.Compile("{{foo}}", arg);

            Assert.Equal("", func(arg));
        }

        [Fact]
        public void It_Can_Retrieve_Values_From_Dynamic()
        {
            dynamic input = new ExpandoObject();
            input.Foo = "Bar";
            input.Number = 1;
            input.Blah = new { String = "Test" };

            var builder = new CompilerSettingsBuilder();
            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var func = stubble.Compile<ExpandoObject>("{{Foo}} {{Number}} {{Blah.String}}", input);

            Assert.Equal("Bar 1 ", func(input));
        }

        [Fact]
        public void It_Can_Retrieve_Values_From_Dynamic_CaseInsensitively()
        {
            dynamic input = new ExpandoObject();
            input.Foo = "Bar";
            input.Number = 1;
            input.Blah = new { String = "Test" };

            var builder = new CompilerSettingsBuilder().SetIgnoreCaseOnKeyLookup(true);
            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var func = stubble.Compile<ExpandoObject>("{{foo}} {{number}}", input);

            var value = func(input);
            Assert.Equal("Bar 1", value);
        }

        [Fact]
        public void It_Should_Throw_On_Data_Miss_Based_On_RenderSettings()
        {
            var input = new
            {
                Foo = "Foo"
            };

            var builder = new CompilerSettingsBuilder();
            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var ex = Assert.Throws<StubbleDataMissException>(() => stubble.Compile("{{Bar}}", input, new CompilationSettings { ThrowOnDataMiss = true }));
            Assert.Equal("'Bar' is undefined.", ex.Message);
            Assert.NotNull(ex.Data["Name"]);
            Assert.NotNull(ex.Data["SkipRecursiveLookup"]);
        }

        [Fact]
        public void You_Should_Be_Able_To_Build_Using_Builder()
        {
            var builder = new StubbleCompilationBuilder();
            builder.Configure(b => b.SetIgnoreCaseOnKeyLookup(true));
            var stubble = builder.Build();

            var input = new { Foo = "Bar" };
            var func = stubble.Compile("{{Foo}}", input);

            Assert.Equal("Bar", func(input));
        }

        [Fact]
        public void You_Should_Be_Able_To_Create_A_Renderer()
        {
            var stubble = new StubbleCompilationRenderer();

            var input = new { Foo = "Bar" };
            var func = stubble.Compile("{{Foo}}", input);

            Assert.Equal("Bar", func(input));
        }

        [Fact]
        public async Task You_Should_Be_Able_To_Compile_Without_An_Example_Object()
        {
            var stubble = new StubbleCompilationRenderer();

            var input = new ExampleClass { Foo = "Bar" };
            var func = stubble.Compile<ExampleClass>("{{Foo}}");
            var funcAsync = await stubble.CompileAsync<ExampleClass>("{{Foo}}");

            Assert.Equal("Bar", func(input));
            Assert.Equal("Bar", funcAsync(input));
        }

        [Fact]
        public async Task You_Should_Be_Able_To_Compile_Without_An_Example_Object_And_CompilationSettings()
        {
            var stubble = new StubbleCompilationRenderer();
            var settings = new CompilationSettings
            {
                ThrowOnDataMiss = true
            };

            var input = new ExampleClass { Foo = "Bar" };
            var func = stubble.Compile<ExampleClass>("{{Foo}}", settings);
            var funcAsync = await stubble.CompileAsync<ExampleClass>("{{Foo}}", settings);

            Assert.Equal("Bar", func(input));
            Assert.Equal("Bar", funcAsync(input));
        }

        [Fact]
        public async Task You_Should_Be_Able_To_Compile_Without_An_Example_Object_With_Partials()
        {
            var stubble = new StubbleCompilationRenderer();
            var partials = new Dictionary<string, string>
            {
                {"Partial", "{{Foo}}"}
            };

            var input = new ExampleClass { Foo = "Bar" };
            var func = stubble.Compile<ExampleClass>("{{> Partial}}", partials);
            var funcAsync = await stubble.CompileAsync<ExampleClass>("{{> Partial}}", partials);

            Assert.Equal("Bar", func(input));
            Assert.Equal("Bar", funcAsync(input));
        }

        [Fact]
        public async Task You_Should_Be_Able_To_Compile_Without_An_Example_Object_With_Partials_And_Settings()
        {
            var stubble = new StubbleCompilationRenderer();
            var settings = new CompilationSettings
            {
                ThrowOnDataMiss = true
            };
            var partials = new Dictionary<string, string>
            {
                {"Partial", "{{Foo}}"}
            };

            var input = new ExampleClass { Foo = "Bar" };
            var func = stubble.Compile<ExampleClass>("{{> Partial}}", partials, settings);
            var funcAsync = await stubble.CompileAsync<ExampleClass>("{{> Partial}}", partials, settings);

            Assert.Equal("Bar", func(input));
            Assert.Equal("Bar", funcAsync(input));
        }

        [Fact]
        public async Task You_Should_Be_Able_To_Compile_With_An_Example_Object_With_Settings()
        {
            var stubble = new StubbleCompilationRenderer();
            var settings = new CompilationSettings
            {
                ThrowOnDataMiss = true
            };

            var input = new ExampleClass { Foo = "Bar" };
            var func = stubble.Compile("{{Foo}}", input, settings);
            var funcAsync = await stubble.CompileAsync("{{Foo}}", input, settings);

            Assert.Equal("Bar", func(input));
            Assert.Equal("Bar", funcAsync(input));
        }

        [Fact]
        public async Task You_Should_Be_Able_To_Compile_With_An_Example_Object_With_Partials_And_Settings()
        {
            var stubble = new StubbleCompilationRenderer();
            var settings = new CompilationSettings
            {
                ThrowOnDataMiss = true
            };
            var partials = new Dictionary<string, string>
            {
                {"Partial", "{{Foo}}"}
            };

            var input = new ExampleClass { Foo = "Bar" };
            var func = stubble.Compile("{{> Partial}}", input, partials, settings);
            var funcAsync = await stubble.CompileAsync("{{> Partial}}", input, partials, settings);

            Assert.Equal("Bar", func(input));
            Assert.Equal("Bar", funcAsync(input));
        }

        [Fact]
        public async Task Unknown_Exceptions_Are_Thrown()
        {
            var partials = new Dictionary<string, string>
            {
                {"Partial", "{{Foo}}"}
            };

            var stubble = new StubbleCompilationBuilder()
                .Configure(configure => { configure.SetTemplateLoader(new DictionaryLoader(partials)); })
                .Build();

            var input = new ExampleClass { Foo = "Bar" };

            Assert.Throws<UnknownTemplateException>(() => stubble.Compile("MissingPartial", input));
            await Assert.ThrowsAsync<UnknownTemplateException>(async () => await stubble.CompileAsync("MissingPartial", input));
        }

        [Fact]
        public void It_Should_Skip_Html_Encoding_With_Setting()
        {
            var stubble = new StubbleCompilationBuilder()
                .Build();

            var obj = new
            {
                Html = "<b>Html</b>"
            };

            var func = stubble.Compile("{{Html}}\n{{{Html}}}", obj, new CompilationSettings
            {
                SkipHtmlEncoding = true
            });

            Assert.Equal("<b>Html</b>\n<b>Html</b>", func(obj));
        }

        [Fact]
        public async Task It_Should_Skip_Html_Encoding_With_Setting_Async()
        {
            var stubble = new StubbleCompilationBuilder()
                .Build();

            var obj = new
            {
                Html = "<b>Html</b>"
            };

            var func = await stubble.CompileAsync("{{Html}}\n{{{Html}}}", obj, new CompilationSettings
            {
                SkipHtmlEncoding = true
            });

            Assert.Equal("<b>Html</b>\n<b>Html</b>", func(obj));
        }

        [Fact]
        public void It_Should_Loop_Dictionary_When_Allowed()
        {
            var stubble = new StubbleCompilationBuilder()
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

            var func = stubble.Compile("{{#Dict}}{{Key}}|{{Value}}.{{/Dict}}", obj);

            Assert.Equal("key1|value1.key2|value2.key3|value3.", func(obj));
        }

        [Fact]
        public void It_Can_Override_Encoding_Function()
        {
            Expression<Func<string, string>> encodingFunc = (str) => str;

            var stubbleBuilder = new StubbleCompilationBuilder()
                .Configure(settings => settings.SetEncodingFunction(encodingFunc))
                .Build();
        }

        [Fact]
        public void It_Should_Throw_When_Ambiguous_Match()
        {
            var stubble = new StubbleCompilationBuilder()
                .Configure(settings =>
                {
                    settings.SetIgnoreCaseOnKeyLookup(true);
                })
                .Build();

            var ex = Assert.Throws<StubbleAmbigousMatchException>(() => stubble.Compile("{{name}}", new { Name = "foo", name = "bar" }));

            Assert.Equal("Ambiguous match found when looking up key: 'name'", ex.Message);
        }

        public static IEnumerable<object[]> Data => new List<SpecTest>
        {
            new SpecTest
            {
                Name = @"Deeply Nested Test",
                Desc = @"Mustache-free templates should render as-is.",
                Data = new {
                    a = new {
                        b = new {
                            c = new {
                                d = new {
                                    e = new {
                                        earlyData = "Not Too Deeply Nested Data",
                                        f = new {
                                            g = new {
                                                h = new {
                                                    i = new {
                                                        j = new {
                                                            k = new {
                                                                l = new {
                                                                    m = new {
                                                                        n = new {
                                                                            o = new {
                                                                                p = new {
                                                                                    q = new {
                                                                                        data = "Very Nested Data"
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Template = @"{{a.b.c.d.e.f.g.h.i.j.k.l.m.n.o.p.q.data}} | {{a.b.c.d.e.earlyData}} == {{#a}} {{#b}} {{#c}} {{#d}} {{#e}} {{#f}} {{#g}} {{#h}} {{#i}} {{#j}} {{#k}} {{#l}} {{#m}} {{#n}} {{#o}} {{#p}} {{#q}} {{> display-data}} {{/q}} {{/p}} {{/o}} {{/n}} {{/m}} {{/l}} {{/k}} {{/j}} {{/i}} {{/h}} {{/g}} {{/f}} {{/e}} {{/d}} {{/c}} {{/b}} {{/a}}",
                Partials = new Dictionary<string, string> {
                    { @"display-data", "{{earlyData}} | {{data}}" }
                },
                ExpectedException = new StubbleException("Cannot call a partial with more than 16 parameters.\nThis is likely due to a large amount of section scopes"),
                Expected = @"Not Too Deeply Nested Data | Very Nested Data == Not Too Deeply Nested Data | Very Nested Data"
            },
            new SpecTest
            {
                Name = "It can render Enumerators",
                Data = new { Items = "abcdefg".ToCharArray().GetEnumerator() },
                Expected = "abcdefg",
                Template = "{{#Items}}{{.}}{{/Items}}"
            },
            new SpecTest
            {
                Name = "It can render Enumerators Twice in the same Context",
                Data = new { Items = "abcdefg".ToCharArray().GetEnumerator() },
                Expected = "abcdefg abcdefg",
                Template = "{{#Items}}{{.}}{{/Items}} {{#Items}}{{.}}{{/Items}}"
            },
            new SpecTest
            {
                Name = "It can render without Data",
                Data = new {},
                Expected = "I have No Data :(",
                Template = "I have No Data :("
            },
            new SpecTest
            {
                Name = "Primatives Are Always Truthy",
                Data = new { MyInt = 1 },
                Template = "{{#MyInt}}That should totally be true{{/MyInt}}",
                Expected = "That should totally be true"
            },
            new SpecTest
            {
                Name = "Empty Partials are Cached Specially",
                Data = new { MyInt = 1 },
                Template = "{{MyInt}}{{>EmptyPartial}}",
                Expected = "1",
                Partials = new Dictionary<string, string>
                {
                    { "EmptyPartial", "" }
                }
            }
        }.Select(s => new[] { s });

        private class ExampleClass
        {
            public string Foo { get; set; }
        }
    }
}
