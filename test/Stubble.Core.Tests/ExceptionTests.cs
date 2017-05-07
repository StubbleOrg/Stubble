// <copyright file="ExceptionTests.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Stubble.Core.Exceptions;
using Xunit;

namespace Stubble.Core.Tests
{
    public class ExceptionTests
    {
        [Fact]
        public void UnknownTemplateExceptions_Constructors_Should_Work()
        {
            Assert.NotNull(new UnknownTemplateException());
            Assert.NotNull(new UnknownTemplateException("Test"));
            Assert.NotNull(new UnknownTemplateException("Test", new Exception("Inner Test")));
        }

        [Fact]
        public void StubbleException_Constructors_Should_Work()
        {
            Assert.NotNull(new StubbleException());
            Assert.NotNull(new StubbleException("Test"));
            Assert.NotNull(new StubbleException("Test", new Exception("Inner Test")));
        }

        [Fact]
        public void StubbleDataMissException_Constructors_Should_Work()
        {
            Assert.NotNull(new StubbleDataMissException());
            Assert.NotNull(new StubbleDataMissException("Test"));
            Assert.NotNull(new StubbleDataMissException("Test", new Exception("Inner Test")));
        }
    }
}
