using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Tests.Fixtures;
using Xunit;

namespace Stubble.Core.Tests
{
    [CollectionDefinition("ContextCollection")]
    public class ContextCollection : ICollectionFixture<ContextTestFixture> { }

    [CollectionDefinition("ChildContextCollection")]
    public class ChildContextCollection : ICollectionFixture<ChildContextTestFixture> { }

    [Collection("ContextCollection")]
    public class ContextTest
    {
        public Context Context;

        public ContextTest(ContextTestFixture fixture)
        {
            Context = fixture.Context;
        }

        [Fact]
        public void It_Can_Lookup_Properties_In_Its_Own_View()
        {
            Assert.Equal("parent", Context.Lookup("Name"));
        }

        [Fact]
        public void It_Can_Lookup_Nested_Properties_In_Its_Own_View()
        {
            Assert.Equal("b", Context.Lookup("A.B"));
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

    [Collection("ChildContextCollection")]
    public class ChildContextTest
    {
        public Context Context;

        public ChildContextTest(ChildContextTestFixture fixture)
        {
            Context = fixture.Context;
        }

        [Fact]
        public void It_Returns_Child_Context()
        {
            Assert.Equal("child", Context.View.Name);
            Assert.Equal("parent", Context.ParentContext.View.Name);
        }

        [Fact]
        public void It_Is_Able_To_Lookup_Properties_Of_Own_View()
        {
            Assert.Equal("child", Context.Lookup("Name"));
        }

        [Fact]
        public void It_Is_Able_To_Lookup_Properties_In_Parents_View()
        {
            Assert.Equal("hi", Context.Lookup("Message"));
        }

        [Fact]
        public void It_Is_Able_To_Lookup_Nested_Properties_Of_Its_Own_View()
        {
            Assert.Equal("d", Context.Lookup("C.D"));
        }

        [Fact]
        public void It_Is_Able_To_Lookup_Nested_Properties_Of_Its_Parents_View()
        {
            Assert.Equal("b", Context.Lookup("A.B"));
        }
    }
}
