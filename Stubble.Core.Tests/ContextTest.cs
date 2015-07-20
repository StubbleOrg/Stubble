using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;
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
        public ContextTestFixture Fixture;

        public ContextTest(ContextTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void It_Can_Lookup_Properties_In_Its_Own_View()
        {
            Assert.Equal("parent", Fixture.Context.Lookup("Name"));
        }

        [Fact]
        public void It_Can_Lookup_Nested_Properties_In_Its_Own_View()
        {
            Assert.Equal("b", Fixture.Context.Lookup("A.B"));
        }

        [Fact]
        public void It_Can_Render_Lambda_Functions()
        {
            var context = new Context(new
            {
                Foo = new Func<object>(() => "TestyTest")
            }, new Registry());
            var output = context.Lookup("Foo");
            var functionOutput = output as Func<object>;
            Assert.Equal("TestyTest", functionOutput.Invoke());
        }

        [Fact]
        public void It_Can_Render_Lambda_Functions_WithArguments()
        {
            var context = new Context(new
            {
                MyData = "Data!",
                Foo = new Func<dynamic, object>((data) => data.MyData)
            }, new Registry());
            var output = context.Lookup("Foo");
            var functionOutput = output as Func<dynamic, object>;

            Assert.Equal("Data!", functionOutput.Invoke(context.View));
        }

        [Fact]
        public void It_Can_Retrieve_Values_From_Objects()
        {
            var context = new Context(new
            {
                MyData = "Data!"
            }, new Registry());
            var output = context.Lookup("MyData");

            Assert.Equal("Data!", output);
        }

        [Fact]
        public void It_Can_Retrieve_Values_From_Dictionary()
        {
            var context = new Context(new Dictionary<string, object>
            {
                { "Foo", "Bar"},
                { "Foo2", 1 }
            }, new Registry());
            var output = context.Lookup("Foo");
            var output2 = context.Lookup("Foo2");

            Assert.Equal("Bar", output);
            Assert.Equal(1, output2);
        }

        [Fact]
        public void It_Can_Retrieve_Values_From_Dynamic()
        {
            dynamic input = new ExpandoObject();
            input.Foo = "Bar";
            input.Number = 1;
            input.Blah = new { String = "Test" };

            var context = new Context(input, new Registry());
            var output = context.Lookup("Foo");
            var output2 = context.Lookup("Number");
            var output3 = context.Lookup("Blah.String");

            Assert.Equal("Bar", output);
            Assert.Equal(1, output2);
            Assert.Equal("Test", output3);
        }

        [Fact]
        public void It_Can_Retrive_Properties_From_Object()
        {
            StronglyTypedTestClass.StaticProperty = 1;
            StronglyTypedTestClass.StaticField = 1;
            var context = new Context(new StronglyTypedTestClass()
            {
                Field = 1,
                Property = 1
            }, new Registry());

            var instanceProperty = context.Lookup("Property");
            var staticProperty = context.Lookup("StaticProperty");
            var instanceField = context.Lookup("Field");
            var staticField = context.Lookup("StaticField");
            var instanceMethodWithoutArgs = context.Lookup("MethodWithoutArgs");
            var instanceMethodWithArgs = context.Lookup("MethodWithArgs");
            var staticMethodWithoutArgs = context.Lookup("StaticMethodWithNoArgs");
            var staticMethodWithArgs = context.Lookup("StaticMethodWithArgs");

            Assert.Equal(1, staticProperty);
            Assert.Equal(1, instanceProperty);
            Assert.Equal(1, instanceField);
            Assert.Equal(1, staticField);
            Assert.Equal(1, instanceMethodWithoutArgs);
            Assert.Equal(1, staticMethodWithoutArgs);
            Assert.Null(instanceMethodWithArgs);
            Assert.Null(staticMethodWithArgs);
        }

        [Fact]
        public void It_Can_Retrive_Properties_From_Object_And_Parent()
        {
            StronglyTypedTestClass.StaticProperty = 1;
            StronglyTypedTestClass.StaticField = 1;
            StronglyTypedChildTestClass.ChildStaticField = 2;
            StronglyTypedChildTestClass.ChildStaticProperty = 2;

            var context = new Context(new StronglyTypedChildTestClass()
            {
                Field = 1,
                Property = 1,
                ChildField = 2,
                ChildProperty = 2
            }, new Registry());

            var parentInstanceProperty = context.Lookup("Property");
            var parentInstanceField = context.Lookup("Field");
            var instanceProperty = context.Lookup("ChildProperty");
            var instanceField = context.Lookup("ChildField");
            var parentInstanceMethodWithoutArgs = context.Lookup("MethodWithoutArgs");
            var parentInstanceMethodWithArgs = context.Lookup("MethodWithArgs");
            var instanceMethodWithoutArgs = context.Lookup("ChildMethodWithoutArgs");

            Assert.Equal(1, parentInstanceProperty);
            Assert.Equal(1, parentInstanceField);
            Assert.Equal(1, parentInstanceMethodWithoutArgs);
            Assert.Equal(2, instanceProperty);
            Assert.Equal(2, instanceField);
            Assert.Equal(2, instanceMethodWithoutArgs);
            Assert.Null(parentInstanceMethodWithArgs);
        }

        [Fact]
        public void It_Is_Truthy_For_Strings()
        {
            Assert.True(Fixture.Context.IsTruthyValue("Yes"));
            Assert.False(Fixture.Context.IsTruthyValue(""));
        }

        [Fact]
        public void It_Can_Use_Truthy_Checks()
        {
            var registry = new Registry(new RegistrySettings
            {
                TruthyChecks = new List<Func<object, bool?>>
                {
                    (val) =>
                    {
                        if (val is string)
                        {
                            return val.Equals("Foo");
                        }
                        return null;
                    },
                    (val) =>
                    {
                        if (val is uint)
                        {
                            return (uint)val > 0;
                        }
                        return null;
                    }
                }
            });

            var context = new Context(new StronglyTypedChildTestClass()
            {
                Field = 1,
                Property = 1,
                ChildField = 2,
                ChildProperty = 2
            }, registry);

            Assert.True(context.IsTruthyValue("Foo"));
            Assert.True(context.IsTruthyValue((uint)5));
            Assert.True(context.IsTruthyValue(true));
        }

        [Fact]
        public void It_Can_Retrieve_Array_Values_By_Index()
        {
            var input = new
            {
                Array = new[] {"Foo", "Bar"}
            };

            var context = new Context(input, new Registry());
            var output = context.Lookup("Array.0");
            Assert.Equal("Foo", output);
        }

        [Fact]
        public void It_Can_Retrieve_Nested_Array_Values_By_Multiple_Indexes()
        {
            var input = new
            {
                Array = new[] {new[] {"Foo"}}
            };

            var context = new Context(input, new Registry());
            var output = context.Lookup("Array.0.0");
            Assert.Equal("Foo", output);
        }

        [Fact]
        public void It_Should_Skip_Indexes_Outside_Of_Array()
        {
            var input = new
            {
                Array = new[] { "Foo", "Bar" }
            };

            var context = new Context(input, new Registry());
            var output = context.Lookup("Array.2");
            var output2 = context.Lookup("Array.10");
            Assert.Null(output);
            Assert.Null(output2);
        }

        [Fact]
        public void It_Should_Skip_If_Index_Isnt_Int()
        {
            var input = new
            {
                Array = new[] { "Foo", "Bar" }
            };

            var context = new Context(input, new Registry());
            var output = context.Lookup("Array.Foo");
            Assert.Null(output);
        }

        [Fact]
        public void It_Should_Allow_Index_Access_On_Lists()
        {
            var input = new
            {
                List = new List<string> { "Foo", "Bar" }
            };

            var context = new Context(input, new Registry());
            var output = context.Lookup("List.0");
            Assert.Equal("Foo", output);
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

    public class StronglyTypedTestClass
    {
        #region Statics
        public static int StaticProperty
        {
            get;
            set;
        }
        public static int StaticField;
        public static int StaticMethodWithNoArgs()
        {
            return 1;
        }
        public static int StaticMethodWithArgs(int i)
        {
            return i;
        }
        #endregion
        #region Instance Variables
        public int Property { get; set; }
        public int Field;
        public int MethodWithoutArgs()
        {
            return 1;
        }
        public int MethodWithArgs(int i)
        {
            return i;
        }
        #endregion
    }

    public class StronglyTypedChildTestClass : StronglyTypedTestClass
    {
        #region Statics
        public static int ChildStaticProperty { get; set; }
        public static int ChildStaticField;
        public static int ChildStaticMethodWithoutArgs()
        {
            return 2;
        }
        #endregion
        #region Instance Variables
        public int ChildProperty { get; set; }
        public int ChildField { get; set; }
        public int ChildMethodWithoutArgs()
        {
            return 2;
        }
        #endregion
    }
}
