// <copyright file="WriterTestFixture.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;
using Stubble.Core.Dev.Settings;

namespace Stubble.Core.Tests.Fixtures
{
    public class WriterTestFixture
    {
        public WriterTestFixture()
        {
            var registry = new Registry();
            Writer = new Writer(new RendererSettingsBuilder().BuildSettings(), registry.TokenMatchRegex, registry.TokenGetters);
        }

        public Writer Writer { get; set; }
    }
}