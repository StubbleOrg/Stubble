// <copyright file="ChildContextTestFixture.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Tests.Fixtures
{
    public class ChildContextTestFixture : ContextTestFixture
    {
        public ChildContextTestFixture()
        {
            Context = Context.Push(new
            {
                Name = "child",
                C = new
                {
                    D = "d"
                }
            });
        }
    }
}
