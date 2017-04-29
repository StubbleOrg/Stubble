// <copyright file="LiteralTag.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Dev.Imported;

namespace Stubble.Core.Dev.Tags
{
    /// <summary>
    /// An inline tag reprsenting a string of characters
    /// </summary>
    public class LiteralTag : MustacheTag
    {
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
        public StringSlice[] Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content of the tag is just whitespace or empty
        /// </summary>
        public bool IsWhitespace { get; set; }

        /// <summary>
        /// Is the tag equal to the other literal tag
        /// </summary>
        /// <param name="other">the object</param>
        /// <returns>If they are equal</returns>
        public bool Equals(LiteralTag other)
        {
            if (other == null)
            {
                return false;
            }

            if (Content != null && other.Content != null)
            {
                if (Content.Length != other.Content.Length)
                {
                    return false;
                }

                for (var i = 0; i < Content.Length; i++)
                {
                    var equal = other.Content[i].Equals(Content[i]);
                    if (!equal)
                    {
                        return false;
                    }
                }
            }

            return !(Content == null & other.Content != null) &&
                   !(Content != null & other.Content == null) &&
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
                hash = (13 * hash) + ContentStartPosition.GetHashCode();
                hash = (13 * hash) + ContentEndPosition.GetHashCode();
                hash = (13 * hash) + Content.GetHashCode();
                hash = (13 * hash) + IsClosed.GetHashCode();
                return hash;
            }
        }
    }
}