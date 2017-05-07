// <copyright file="TypeBySubclassAndAssignableTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Stubble.Core.Classes;
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
                typeof(A),
                typeof(B)
            };
            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable()).ToList();

            var rawTokenIndex = orderedList.IndexOf(typeof(B));
            var parserOutputIndex = orderedList.IndexOf(typeof(A));

            Assert.True(rawTokenIndex < parserOutputIndex);
        }

        [Fact]
        public void It_Should_Put_Subclasses_Before_Parent_Regardless_Of_Starting_Position()
        {
            var list = new List<Type>
            {
                typeof(B),
                typeof(A)
            };
            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable()).ToList();

            var rawTokenIndex = orderedList.IndexOf(typeof(B));
            var parserOutputIndex = orderedList.IndexOf(typeof(A));

            Assert.True(rawTokenIndex < parserOutputIndex);
        }

        [Fact]
        public void Types_Should_Come_Before_AssignableTypes()
        {
            var list = new List<Type>
            {
                typeof(IEnumerable),
                typeof(JToken)
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
                typeof(JToken),
                typeof(IEnumerable)
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

        private class A
        {
            public string PropertyA { get; set; }
        }

        private class B : A
        {
            public string PropertyB { get; set; }
        }
    }
}
