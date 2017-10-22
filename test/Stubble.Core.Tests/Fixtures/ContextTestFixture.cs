// <copyright file="ContextTestFixture.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;
using Stubble.Core.Contexts;
using Stubble.Core.Settings;

namespace Stubble.Core.Tests.Fixtures
{
    public class ContextTestFixture
    {
        public ContextTestFixture()
        {
            Context = new Context(
                new
                {
                    Name = "parent",
                    Message = "hi",
                    A = new
                    {
                        B = "b"
                    }
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());
        }

        public Context Context { get; set; }
    }
}
