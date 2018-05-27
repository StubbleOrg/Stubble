// <copyright file="InvertedSectionToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Tokens
{
    /// <summary>
    /// A block tag representing an inverted section token
    /// </summary>
    public class InvertedSectionToken : BlockToken<InvertedSectionToken>
    {
        /// <summary>
        /// Gets or sets the sections name
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the starting position of the tag
        /// </summary>
        public int StartPosition { get; set; }

        /// <summary>
        /// Gets or sets the end position of the tag
        /// </summary>
        public int EndPosition { get; set; }

        /// <inheritdoc/>
        public override string Identifier => SectionName;

        /// <inheritdoc/>
        public override bool Equals(InvertedSectionToken other)
        {
            if (other == null)
            {
                return false;
            }

            if (Children != null && other.Children != null)
            {
                if (Children.Count != other.Children.Count)
                {
                    return false;
                }

                for (var i = 0; i < Children.Count; i++)
                {
                    var equal = other.Children[i].Equals(Children[i]);
                    if (!equal)
                    {
                        return false;
                    }
                }
            }

            return !(Children == null & other.Children != null) &&
                   !(Children != null & other.Children == null) &&
                   other.IsClosed == IsClosed &&
                   other.SectionName == SectionName &&
                   other.StartPosition == StartPosition &&
                   other.EndPosition == EndPosition;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is InvertedSectionToken a && Equals(a);
        }

        /// <summary>
        /// Gets the hash code for the tag
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ StartPosition;
                hashCode = (hashCode * 397) ^ EndPosition;
                hashCode = (hashCode * 397) ^ (SectionName?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
