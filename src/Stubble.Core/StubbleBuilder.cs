// <copyright file="StubbleBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;

namespace Stubble.Core
{
    /// <summary>
    /// Represents the primary builder class for creating <see cref="StubbleStringRenderer"/> instances
    /// </summary>
    public sealed class StubbleBuilder : StubbleBuilder<StubbleStringRenderer>
    {
        /// <summary>
        /// Builds a <see cref="StubbleStringRenderer"/> instance with the initalised settings
        /// </summary>
        /// <returns>Returns a <see cref="StubbleStringRenderer"/> with the initalised settings</returns>
        public override StubbleStringRenderer Build()
        {
            return new StubbleStringRenderer(new Registry(BuildRegistrySettings()));
        }
    }
}
