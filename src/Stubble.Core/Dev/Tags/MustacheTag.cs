// <copyright file="MustacheTag.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Dev.Tags
{
    /// <summary>
    /// An abstract class representing any MustacheTag
    /// </summary>
    public abstract class MustacheTag
    {
        /// <summary>
        /// Gets or sets a value indicating whether the tag has been closed
        /// </summary>
        public bool IsClosed { get; set; }
    }
}
