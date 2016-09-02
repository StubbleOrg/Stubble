// <copyright file="BlockTag.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Stubble.Core.Dev.Parser;

namespace Stubble.Core.Dev.Tags
{
    /// <summary>
    /// The base class for a non generic BlockTag
    /// </summary>
    public abstract class BlockTag : MustacheTag
    {
        /// <summary>
        /// Gets the identifier for the block
        /// </summary>
        public abstract string Identifier { get; }

        /// <summary>
        /// Gets or sets the parser used to parse the block
        /// </summary>
        public BlockParser Parser { get; set; }

        /// <summary>
        /// Gets or sets the children of the block.
        /// </summary>
        public List<MustacheTag> Children { get; set; } = new List<MustacheTag>();
    }
}
