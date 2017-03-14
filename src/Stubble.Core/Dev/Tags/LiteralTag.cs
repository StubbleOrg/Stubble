// <copyright file="LiteralTag.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Dev.Tags
{
    /// <summary>
    /// An inline tag reprsenting a string of characters
    /// </summary>
    public class LiteralTag : InlineTag<LiteralTag>
    {
        /// <summary>
        /// Gets a value indicating whether the content of the tag is just whitespace
        /// </summary>
        public bool IsWhitespace => string.IsNullOrWhiteSpace(Content);

        /// <inheritdoc/>
        public override bool Equals(LiteralTag other)
        {
            if (other == null)
            {
                return false;
            }

            return other.Content == Content &&
                   other.TagStartPosition == TagStartPosition &&
                   other.TagEndPosition == TagEndPosition &&
                   other.ContentStartPosition == ContentStartPosition &&
                   other.ContentEndPosition == ContentEndPosition &&
                   other.IsClosed == IsClosed;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var a = obj as LiteralTag;
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