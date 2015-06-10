using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Stubble.Core.Tests
{
    public class ContextTest
    {
        [Fact]
        public void It_Can_Get_Value_From_An_Object_One_Level_Deep()
        {
            var context = new Context(new
            {
                Foo = "Bar"
            });
            var output = context.Lookup("Foo");
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Get_Value_From_An_Object_Multiple_Levels_Deep()
        {
            var context = new Context(new
            {
                Foo = new
                {
                    Foo = new
                    {
                        Foo = new
                        {
                            Foo = "Bar"
                        }
                    }
                }
            });
            var output = context.Lookup("Foo.Foo.Foo.Foo");
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Can_Render_Lambda_Functions()
        {
            var context = new Context(new
            {
                Foo = new Func<string>(() => "TestyTest")
            });
            var output = context.Lookup("Foo");
            Assert.Equal("TestyTest", output);
        }

        [Fact]
        public void It_Can_Render_Lambda_Functions_WithArguments()
        {
            var context = new Context(new
            {
                MyData = "Data!",
                Foo = new Func<dynamic, string>((data) => data.MyData)
            });
            var output = context.Lookup("Foo");
            Assert.Equal("Data!", output);
        }
    }
}
