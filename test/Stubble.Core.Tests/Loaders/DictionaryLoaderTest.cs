// <copyright file="DictionaryLoaderTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Classes.Loaders;
using Xunit;

namespace Stubble.Core.Tests.Loaders
{
    public class DictionaryLoaderTest
    {
        [Fact]
        public void It_Should_Take_An_Array_Of_Templates()
        {
            var loader = new DictionaryLoader(new Dictionary<string, string>
            {
                { "Foo", "I'm {{Foo}}" },
                { "Bar", "I'm {{Bar}}" }
            });

            Assert.Equal(2, loader.TemplateCache.Count);
        }

        [Fact]
        public void It_Should_Create_A_Clone_Of_Passed_Values()
        {
            var arr = new Dictionary<string, string>
            {
                {"Foo", "I'm {{Foo}}"},
                {"Bar", "I'm {{Bar}}"}
            };
            var loader = new DictionaryLoader(arr);
            Assert.Equal(2, loader.TemplateCache.Count);
            arr.Add("FooBar", "{{Foo}}{{Bar}}");
            Assert.Equal(2, loader.TemplateCache.Count);
        }

        [Fact]
        public void It_Should_Load_Items_From_Template_If_Exists()
        {
            var loader = new DictionaryLoader(new Dictionary<string, string>
            {
                { "Foo", "I'm {{Foo}}" },
                { "Bar", "I'm {{Bar}}" }
            });

            Assert.Equal("I'm {{Foo}}", loader.Load("Foo"));
            Assert.Null(loader.Load("Foo2"));
        }
    }
}
