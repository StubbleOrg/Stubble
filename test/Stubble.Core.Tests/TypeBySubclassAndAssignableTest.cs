// <copyright file="TypeBySubclassAndAssignableTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
                typeof(B),
                typeof(A),
            };

            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.Default).ToList();

            orderedList.Should().ContainInOrder(new[]
            {
                typeof(B),
                typeof(A),
            });
        }

        [Fact]
        public void It_Should_Put_Subclasses_Before_Parent_Regardless_Of_Starting_Position()
        {
            var list = new List<Type>
            {
                typeof(B),
                typeof(A)
            };

            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.Default).ToList();

            orderedList.Should().ContainInOrder(new[]
            {
                typeof(B),
                typeof(A),
            });
        }

        [Fact]
        public void Types_Should_Come_Before_AssignableTypes()
        {
            var list = new List<Type>
            {
                typeof(IEnumerable),
                typeof(List<string>)
            };
            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.Default).ToList();

            orderedList.Should().ContainInOrder(new[]
            {
                typeof(List<string>),
                typeof(IEnumerable),
            });
        }

        [Fact]
        public void Types_Should_Come_Before_AssignableTypes_Regardless_Of_Order()
        {
            var list = new List<Type>
            {
                typeof(List<string>),
                typeof(IEnumerable)
            };
            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.Default).ToList();

            orderedList.Should().ContainInOrder(new[]
            {
                typeof(List<string>),
                typeof(IEnumerable),
            });
        }

        [Fact]
        public void It_Should_Perform_Standard_Equality()
        {
            var comparer = TypeBySubclassAndAssignableImpl.Default;
            Assert.Equal(0, comparer.Compare(null, null));
            Assert.Equal(1, comparer.Compare(null, typeof(string)));
            Assert.Equal(-1, comparer.Compare(typeof(string), null));
        }

        [Fact]
        public void NonInterface_Should_Come_Before_Interface()
        {
            var list = new List<Type>
            {
                typeof(JArray),
                typeof(IList),
                typeof(IDictionary<string, object>),
            };

            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.Default).ToList();

            orderedList.Should().ContainInOrder(new[]
            {
                typeof(JArray),
                typeof(IList),
                typeof(IDictionary<string, object>),
            });
        }

        [Fact]
        public void NonInterface_Should_Come_Before_Interface_Regardless_Of_Order()
        {
            var list = new List<Type>
            {
                typeof(IList),
                typeof(IDictionary<string, object>),
                typeof(JArray),
            };

            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.Default).ToList();

            orderedList.Should().ContainInOrder(new[]
            {
                typeof(JArray),
                typeof(IList),
                typeof(IDictionary<string, object>),
            });
        }

        [Fact]
        public void Object_Should_Be_Put_At_Last()
        {
            var list = new List<Type>
            {
                typeof(IList),
                typeof(IDictionary<string, object>),
                typeof(JArray),
                typeof(object),
            };

            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.Default).ToList();

            orderedList.Should().ContainInOrder(new[]
            {
                typeof(JArray),
                typeof(IList),
                typeof(IDictionary<string, object>),
                typeof(object),
            });
        }

        [Fact]
        public void Object_Should_Be_Put_At_Last_Regardless_Of_Order()
        {
            var list = new List<Type>
            {
                typeof(object),
                typeof(IList),
                typeof(IDictionary<string, object>),
                typeof(JArray),
            };

            var orderedList = list.OrderBy(x => x, TypeBySubclassAndAssignableImpl.Default).ToList();

            orderedList.Should().ContainInOrder(new[]
            {
                typeof(JArray),
                typeof(IList),
                typeof(IDictionary<string, object>),
                typeof(object),
            });
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
