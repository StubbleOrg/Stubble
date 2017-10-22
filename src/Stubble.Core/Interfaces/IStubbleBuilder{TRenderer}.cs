// <copyright file="IStubbleBuilder{TRenderer}.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Interfaces
{
    /// <summary>
    /// A non-generic interface for IStubbleBuilders
    /// </summary>
    /// <typeparam name="TRenderer">The type of renderer built</typeparam>
    public interface IStubbleBuilder<TRenderer>
    {
        /// <summary>
        /// Builds and configures the renderer
        /// </summary>
        /// <returns>The built renderer</returns>
        TRenderer Build();
    }
}
