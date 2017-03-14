// <copyright file="InterpolationTag.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes.Tokens.Interface;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Tags
{
    /// <summary>
    /// An inline tag representing an InterpolationTag
    /// </summary>
    public class InterpolationTag : InlineTag<InterpolationTag>, INonSpace
    {
        /// <summary>
        /// Gets or sets a value indicating whether the contents should be escaped
        /// </summary>
        public bool EscapeResult { get; set; }

        /// <inheritdoc/>
        public override bool Equals(InterpolationTag other)
        {
            if (other == null)
            {
                return false;
            }

            return other.EscapeResult == EscapeResult &&
                   other.Content == Content &&
                   other.TagStartPosition == TagStartPosition &&
                   other.TagEndPosition == TagEndPosition &&
                   other.ContentStartPosition == ContentStartPosition &&
                   other.ContentEndPosition == ContentEndPosition &&
                   other.IsClosed == IsClosed;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var a = obj as InterpolationTag;
            return a != null && Equals(a);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) + EscapeResult.GetHashCode();
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