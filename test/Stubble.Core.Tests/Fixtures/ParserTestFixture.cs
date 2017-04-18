// <copyright file="ParserTestFixture.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;

namespace Stubble.Core.Tests.Fixtures
{
    public class ParserTestFixture
    {
        public ParserTestFixture()
        {
            var registry = new Registry();
            Parser = new Parser(registry.TokenMatchRegex, registry.TokenGetters);
        }

        public Parser Parser { get; set; }
    }
}
