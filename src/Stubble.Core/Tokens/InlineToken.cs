// <copyright file="InlineToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Imported;

namespace Stubble.Core.Tokens
{
    /// <summary>
    /// A base class for inline tags
    /// </summary>
    public abstract class InlineToken : MustacheToken
    {
        /// <summary>
        /// Gets or sets the tags start position
        /// </summary>
        public int TagStartPosition { get; set; }

        /// <summary>
        /// Gets or sets the tags end position
        /// </summary>
        public int TagEndPosition { get; set; }

        /// <summary>
        /// Gets or sets the tags content start position
        /// </summary>
        public int ContentStartPosition { get; set; }

        /// <summary>
        /// Gets or sets the tags content end position
        /// </summary>
        public int ContentEndPosition { get; set; }

        /// <summary>
        /// Gets or sets the tag content
        /// </summary>
        public StringSlice Content { get; set; }
    }
}