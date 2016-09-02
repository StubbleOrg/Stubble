// <copyright file="IStubbleBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;

namespace Stubble.Core.Interfaces
{
    /// <summary>
    /// A non-generic interface for IStubbleBuilders
    /// </summary>
    public interface IStubbleBuilder
    {
        /// <summary>
        /// Fill the builders internal store with those from a <see cref="RegistrySettings"/>
        /// instance
        /// </summary>
        /// <param name="settings">The settings to fill from</param>
        void FillFromRegistrySettings(RegistrySettings settings);
    }
}