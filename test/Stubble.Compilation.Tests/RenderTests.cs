using Stubble.Compilation.Settings;
using Stubble.Core.Exceptions;
using Stubble.Test.Shared.Spec;
using System.Collections.Generic;
using System.Linq;
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
                    var output = data.Partials != null ? stubble.Compile(data.Template, data.Data, data.Partials) : stubble.Compile(data.Template, data.Data);

                    var outputResult = output(data.Data);
                });

                Assert.Equal(data.ExpectedException.Message, ex.Message);
            }
            else
            {
                var output = data.Partials != null ? stubble.Compile(data.Template, data.Data, data.Partials) : stubble.Compile(data.Template, data.Data);
                var outputResult = output(data.Data);

                Assert.Equal(data.Expected, outputResult);
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
            }
        }.Select(s => new[] { s });
    }
}
