using Stubble.Compilation.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Stubble.Compilation.Tests
{
    public class ContextTests
    {
        [Fact]
        public void It_Can_Use_Truthy_Checks()
        {
            var builder = new CompilerSettingsBuilder();

            builder.AddTruthyCheck<uint>(val => val > 0);

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var obj = new
            {
                Foo = (uint)10,
                Bar = "Bar"
            };

            var func = stubble.Compile("{{#Foo}}{{Bar}}{{/Foo}}", obj);

            Assert.Equal("Bar", func(obj));
            Assert.Equal("", func(new
            {
                Foo = (uint)0,
                Bar = "Boo"
            }));
        }

        [Fact]
        public void It_Should_Skip_Indexes_Outside_Of_Array()
        {
            var input = new
            {
                Array = new[] { "Foo", "Bar" }
            };

            var builder = new CompilerSettingsBuilder();
            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var func = stubble.Compile("{{Array.2}}", input);
            var func2 = stubble.Compile("{{Array.10}}", input);

            Assert.Equal("", func(input));
            Assert.Equal("", func2(input));
        }

        [Fact]
        public void CompilationRenderer_ItShouldAllowMultipleTruthyChecks()
        {
            var builder = new CompilerSettingsBuilder();

            builder.AddTruthyCheck<string>(val => !val.Equals("Bar"));
            builder.AddTruthyCheck<string>(val => !val.Equals("Boo"));

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var obj = new
            {
                Foo = "Bar",
                Bar = "Display Me"
            };

            var func = stubble.Compile("{{#Foo}}{{Bar}}{{/Foo}}", obj);

            Assert.Equal("", func(obj));
            Assert.Equal("", func(new
            {
                Foo = "Boo",
                Bar = "Display Me"
            }));

            Assert.Equal("", func(new
            {
                Foo = "false",
                Bar = "Display Me"
            }));

            Assert.Equal("", func(new
            {
                Foo = (string)null,
                Bar = "Display Me"
            }));
        }
    }
}
