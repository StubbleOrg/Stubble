using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Tokens;
using Xunit;

namespace Stubble.Core.Tests
{
    public class TypeBySubclassAndAssignableTest
    {
        [Fact]
        public void It_Should_Put_Subclasses_Before_Parent()
        {
            var list = new List<Type>
            {
                typeof (ParserOutput),
                typeof (RawValueToken)
            };
            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable()).ToList();


            var rawTokenIndex = orderedList.IndexOf(typeof(RawValueToken));
            var parserOutputIndex = orderedList.IndexOf(typeof (ParserOutput));

            Assert.True(rawTokenIndex < parserOutputIndex);
        }

        [Fact]
        public void It_Should_Put_Subclasses_Before_Parent_Regardless_Of_Starting_Position()
        {
            var list = new List<Type>
            {
                typeof (RawValueToken),
                typeof (ParserOutput)
            };
            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable()).ToList();

            var rawTokenIndex = orderedList.IndexOf(typeof(RawValueToken));
            var parserOutputIndex = orderedList.IndexOf(typeof(ParserOutput));

            Assert.True(rawTokenIndex < parserOutputIndex);
        }

        [Fact]
        public void Types_Should_Come_Before_AssignableTypes()
        {
            var list = new List<Type>
            {
                typeof (IEnumerable),
                typeof (JToken)
            };
            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable()).ToList();

            var enumerableIndex = orderedList.IndexOf(typeof(IEnumerable));
            var jtokenIndex = orderedList.IndexOf(typeof(JToken));

            Assert.True(jtokenIndex < enumerableIndex);
        }

        [Fact]
        public void Types_Should_Come_Before_AssignableTypes_Regardless_Of_Order()
        {
            var list = new List<Type>
            {
                typeof (JToken),
                typeof (IEnumerable)
            };
            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable()).ToList();

            var enumerableIndex = orderedList.IndexOf(typeof(IEnumerable));
            var jtokenIndex = orderedList.IndexOf(typeof(JToken));

            Assert.True(jtokenIndex < enumerableIndex);
        }

        [Fact]
        public void It_Should_Perform_Standard_Equality()
        {
            var comparer = TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable();
            Assert.Equal(0, comparer.Compare(null, null));
            Assert.Equal(1, comparer.Compare(null, typeof(string)));
            Assert.Equal(-1, comparer.Compare(typeof(string), null));
        }
    }
}
