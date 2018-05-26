// <copyright file="StubbleCompilationBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Stubble.Compilation.Settings;
using Stubble.Core.Interfaces;

namespace Stubble.Compilation.Builders
{
    /// <summary>
    /// A builder for configuring and building a <see cref="StubbleCompilationRenderer"/>
    /// </summary>
    public sealed class StubbleCompilationBuilder : IStubbleBuilder<StubbleCompilationRenderer>
    {
        /// <summary>
        /// Gets the action for configuring settings for the renderer
        /// </summary>
        internal Action<CompilerSettingsBuilder> ConfigureSettings { get; private set; }

        /// <inheritdoc/>
        public StubbleCompilationRenderer Build()
        {
            var builder = new CompilerSettingsBuilder();
            ConfigureSettings?.Invoke(builder);
            return new StubbleCompilationRenderer(builder.BuildSettings());
        }

        /// <summary>
        /// Configures the builder with the provided action
        /// </summary>
        /// <param name="configureSettings">The action to configure the builder with</param>
        /// <returns>The builder to continue chaining with</returns>
        public StubbleCompilationBuilder Configure(Action<CompilerSettingsBuilder> configureSettings)
        {
            ConfigureSettings = configureSettings;
            return this;
        }
    }
}
