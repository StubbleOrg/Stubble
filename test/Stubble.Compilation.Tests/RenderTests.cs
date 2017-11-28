using Stubble.Compilation.Settings;
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
        public void CompilationRendererSpecTest(SpecTest data)
        {
            var builder = new CompilerSettingsBuilder();

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());
            var output = data.Partials != null ? stubble.Compile(data.Template, data.Data, data.Partials) : stubble.Compile(data.Template, data.Data);

            var outputResult = output(data.Data);

            Assert.Equal(data.Expected, outputResult);
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
                Expected = @"Not Too Deeply Nested Data | Very Nested Data == Not Too Deeply Nested Data | Very Nested Data"
            }
        }.Select(s => new[] { s });
    }
}
