// <copyright file="BlockToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Parser.Interfaces;

namespace Stubble.Core.Tokens
{
    /// <summary>
    /// The base class for a non generic BlockTag
    /// </summary>
    public abstract class BlockToken : MustacheToken
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
        public List<MustacheToken> Children { get; set; } = new List<MustacheToken>();

        /// <summary>
        /// Gets or sets the tags content start position
        /// </summary>
        public int ContentStartPosition { get; set; }

        /// <summary>
        /// Gets or sets the tags content end position
        /// </summary>
        public int ContentEndPosition { get; set; }
    }
}
