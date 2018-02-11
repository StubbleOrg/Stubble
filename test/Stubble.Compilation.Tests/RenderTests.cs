using Stubble.Compilation.Settings;
using Stubble.Core.Exceptions;
using Stubble.Test.Shared.Spec;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
            }
        }.Select(s => new[] { s });
    }
}
