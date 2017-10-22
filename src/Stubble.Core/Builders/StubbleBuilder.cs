// <copyright file="StubbleBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Stubble.Core.Interfaces;
using Stubble.Core.Settings;

namespace Stubble.Core.Builders
{
    /// <summary>
    /// A builder for configuring and building a <see cref="StubbleVisitorRenderer"/>
    /// </summary>
    public sealed class StubbleBuilder : IStubbleBuilder<StubbleVisitorRenderer>
    {
        /// <summary>
        /// Gets the action for configuring settings for the renderer
        /// </summary>
        internal Action<RendererSettingsBuilder> ConfigureSettings { get; private set; }

        /// <inheritdoc/>
        public StubbleVisitorRenderer Build()
        {
            var builder = new RendererSettingsBuilder();
            ConfigureSettings?.Invoke(builder);
            return new StubbleVisitorRenderer(builder.BuildSettings());
        }

        /// <summary>
        /// Configures the builder with the provided action
        /// </summary>
        /// <param name="configureSettings">The action to configure the builder with</param>
        /// <returns>The builder to continue chaining with</returns>
        public StubbleBuilder Configure(Action<RendererSettingsBuilder> configureSettings)
        {
            this.ConfigureSettings = configureSettings;
            return this;
        }
    }
}
