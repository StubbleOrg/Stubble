// <copyright file="IStubbleBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;
using Stubble.Core.Dev.Settings;

namespace Stubble.Core.Interfaces
{
    /// <summary>
    /// A non-generic interface for IStubbleBuilders
    /// </summary>
    public interface IStubbleBuilder
    {
        /// <summary>
        /// Set the new stubble builders internal state to that of an existing settings builder
        /// instance
        /// </summary>
        /// <param name="settingsBuilder">The builder to set as the child builder</param>
        void SetRendererSettings(RendererSettingsBuilder settingsBuilder);
    }
}