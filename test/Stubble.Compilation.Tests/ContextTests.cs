using System;
using System.Data;
using System.Linq.Expressions;
using Stubble.Compilation.Settings;
using Xunit;
using System.Linq;

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
