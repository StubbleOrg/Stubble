// <copyright file="IStubbleBuilder{T}.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Settings;

namespace Stubble.Core.Interfaces
{
    /// <summary>
    /// Represents an interface for the stubble builder for initalizing <see cref="T:T"/> instances
    /// </summary>
    /// <typeparam name="T">The renderer instance this builder builds</typeparam>
    public interface IStubbleBuilder<out T> : IStubbleBuilder, IRendererSettingsBuilder<IStubbleBuilder<T>>
    {
        /// <summary>
        /// Builds a <see cref="IStubbleBuilder"/> instance with the initalised settings
        /// </summary>
        /// <returns>Returns a <see cref="IStubbleBuilder"/> with the initalised settings</returns>
        T Build();

        /// <summary>
        /// Converts the current builder into one that returns a different renderer
        /// </summary>
        /// <typeparam name="T1">The type of the new builder</typeparam>
        /// <returns>The existing builder returning the new type</returns>
        T1 SetBuilderType<T1>()
            where T1 : IStubbleBuilder, new();
    }
}
