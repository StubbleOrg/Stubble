// <copyright file="WriterTestFixture.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Tests.Fixtures
{
    public class WriterTestFixture
    {
        public WriterTestFixture()
        {
            Writer = new Writer();
        }

        public Writer Writer { get; set; }
    }
}