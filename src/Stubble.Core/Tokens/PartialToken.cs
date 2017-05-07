// <copyright file="PartialToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Stubble.Core.Tokens
{
    /// <summary>
    /// An inline tag representing a partial template
    /// </summary>
    public class PartialToken : InlineToken<PartialToken>
    {
        /// <summary>
        /// Gets or sets the line indent for the partial tag
        /// </summary>
        public int LineIndent { get; set; }

        /// <inheritdoc/>
        public override bool Equals(PartialToken other)
        {
            if (other == null)
            {
                return false;
            }

            return other.Content.Equals(Content) &&
                   other.TagStartPosition == TagStartPosition &&
                   other.TagEndPosition == TagEndPosition &&
                   other.ContentStartPosition == ContentStartPosition &&
                   other.ContentEndPosition == ContentEndPosition &&
                   other.IsClosed == IsClosed;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var a = obj as PartialToken;
            return a != null && Equals(a);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) + TagStartPosition.GetHashCode();
                hash = (13 * hash) + TagEndPosition.GetHashCode();
                hash = (13 * hash) + ContentStartPosition.GetHashCode();
                hash = (13 * hash) + ContentEndPosition.GetHashCode();
                hash = (13 * hash) + Content.GetHashCode();
                hash = (13 * hash) + IsClosed.GetHashCode();
                return hash;
            }
        }
    }
}