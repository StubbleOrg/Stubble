// <copyright file="DelimiterToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Stubble.Core.Tokens
{
    /// <summary>
    /// An inline tag reprsenting a tag delimiter change
    /// </summary>
    public class DelimiterToken : InlineToken<DelimiterToken>
{
        /// <summary>
        /// Gets or sets the starting tag delimiters
        /// </summary>
        public string StartTag { get; set; }

        /// <summary>
        /// Gets or sets the ending tag delimiters
        /// </summary>
        public string EndTag { get; set; }

        /// <inheritdoc/>
        public override bool Equals(DelimiterToken other)
        {
            if (other == null)
            {
                return false;
            }

            return other.StartTag == StartTag &&
                   other.EndTag == EndTag &&
                   other.Content.Equals(Content) &&
                   other.TagStartPosition == TagStartPosition &&
                   other.TagEndPosition == TagEndPosition &&
                   other.ContentStartPosition == ContentStartPosition &&
                   other.ContentEndPosition == ContentEndPosition &&
                   other.IsClosed == IsClosed;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var a = obj as DelimiterToken;
            return a != null && Equals(a);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) + StartTag.GetHashCode();
                hash = (13 * hash) + EndTag.GetHashCode();
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