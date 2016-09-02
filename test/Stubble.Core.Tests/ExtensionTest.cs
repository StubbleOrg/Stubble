// <copyright file="ExtensionTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Helpers;
using Xunit;

namespace Stubble.Core.Tests
{
    public class ExtensionTest
    {
        [Fact]
        public void It_Can_Slice_Strings()
        {
            var sliced = "I'm a String".Slice(0, 3);
            Assert.Equal("I'm", sliced);
            var slicedAgain = "I'm a String".Slice(0, -7);
            Assert.Equal("I'm a", slicedAgain);
        }
    }
}
