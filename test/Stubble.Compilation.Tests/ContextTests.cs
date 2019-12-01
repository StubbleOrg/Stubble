using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Stubble.Compilation.Settings;
using Xunit;

namespace Stubble.Compilation.Tests
{
    public class ContextTests
    {
        [Theory]
        [InlineData(10u, "Bar")]
        [InlineData(0u, "")]
        public void It_Can_Use_Truthy_Checks(uint fooValue, string expectedResult)
        {
            var builder = new CompilerSettingsBuilder();

            builder.AddTruthyCheck<uint>(val => val > 0);

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var obj = new
            {
                Foo = fooValue,
                Bar = "Bar"
            };

            var func = stubble.Compile("{{#Foo}}{{Bar}}{{/Foo}}", obj);

            Assert.Equal(expectedResult, func(obj));
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

        [Theory]
        [InlineData("Bar", "")]
        [InlineData("Boo", "")]
        [InlineData("false", "")]
        [InlineData("0", "")]
        [InlineData(null, "")]
        [InlineData("true", "Hello World")]
        [InlineData("1", "Hello World")]
        public void CompilationRenderer_ItShouldAllowMultipleTruthyChecks(string fooValue, string result)
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

            var renderResult = func(new
            {
                Foo = fooValue,
                Bar = "Hello World"
            });

            Assert.Equal(result, renderResult);
        }

        [Fact]
        public void ComplicationRenderer_ItShouldAllowCustomValueGetters()
        {
            var builder = new CompilerSettingsBuilder();

            builder.AddValueGetter<DifficultClass>((type, instance, key, ignoreCase) =>
            {
                return Expression.Call(
                    instance,
                    typeof(DifficultClass).GetMethod("GetValue"),
                    Expression.Constant(key),
                    Expression.Constant(ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
            });

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var obj = new DifficultClass("Foo", "Bar");

            var func = stubble.Compile("{{_innerVal}} {{_innerVal2}}", obj);

            Assert.Equal("Foo Bar", func(obj));
        }

        [Fact]
        public void ComplicationRenderer_ItShouldAllowCustomEnumerationConverters()
        {
            var builder = new CompilerSettingsBuilder();

            builder.AddValueGetter<DataRow>((type, instance, key, ignoreCase) =>
            {
                var contains = Expression.Call(Expression.Property(
                    Expression.Property(instance, nameof(DataRow.Table)),
                    nameof(DataTable.Columns)),
                    typeof(DataColumnCollection).GetMethod(nameof(DataColumnCollection.Contains)),
                    Expression.Constant(key));

                return Expression.Condition(contains,
                   Expression.Property(instance, "Item", Expression.Constant(key)),
                   Expression.Constant(null)
                );
            });
            builder.AddEnumerationConverter((DataTable dt) => dt.Rows.Cast<DataRow>());

            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var obj = new DataTable();
            obj.Columns.Add(new DataColumn("Column1"));
            obj.Columns.Add(new DataColumn("Column2"));
            obj.Columns.Add(new DataColumn("Column3"));

            obj.Rows.Add(1, "foo", "bar");
            obj.Rows.Add(2, "foo", "bar");
            obj.Rows.Add(3, "foo", "bar");

            var func = stubble.Compile("{{#dt}}{{Column1}} {{Column2}}.\n{{/dt}}", new { dt = obj });

            Assert.Equal(@"1 foo.
2 foo.
3 foo.
", func(new { dt = obj }));
        }

        [Fact]
        public void It_Should_Not_Render_Falsey_Int() => CheckNoResultForFalseyValue(0);

        [Fact]
        public void It_Should_Not_Render_Falsey_Long() => CheckNoResultForFalseyValue(0L);

        [Fact]
        public void It_Should_Not_Render_Falsey_Decimal() => CheckNoResultForFalseyValue(0m);

        [Fact]
        public void It_Should_Not_Render_Falsey_Float() => CheckNoResultForFalseyValue(0f);

        [Fact]
        public void It_Should_Not_Render_Falsey_Double() => CheckNoResultForFalseyValue(0d);

        private void CheckNoResultForFalseyValue<T>(T value)
        {
            var builder = new CompilerSettingsBuilder();
            var stubble = new StubbleCompilationRenderer(builder.BuildSettings());

            var obj = new
            {
                Val = value
            };

            var func = stubble.Compile("{{#Val}}Oh Hi{{/Val}}", obj);
            var result = func(obj);

            Assert.Equal("", result);
        }

        private class DifficultClass
        {
            private readonly string _innerVal;
            private readonly string _innerVal2;

            public DifficultClass(string val, string val2)
            {
                _innerVal = val;
                _innerVal2 = val2;
            }

            public object GetValue(string value, StringComparison comparison)
            {
                if (value.Equals(nameof(_innerVal), comparison))
                {
                    return _innerVal;
                }
                if (value.Equals(nameof(_innerVal2), comparison))
                {
                    return _innerVal2;
                }

                return null;
            }
        }
    }
}
