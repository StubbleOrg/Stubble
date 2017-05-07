// <copyright file="Tags.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Stubble.Core.Classes.Exceptions;

namespace Stubble.Core.Classes
{
    /// <summary>
    /// Represents a set of tags used to identify tokens
    /// </summary>
    public class Tags : IEquatable<Tags>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tags"/> class.
        /// </summary>
        /// <param name="startTag">Start tag</param>
        /// <param name="endTag">End tag</param>
        public Tags(string startTag, string endTag)
        {
            // TODO: Check tags do not have spaces
            StartTag = startTag;
            EndTag = endTag;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tags"/> class.
        /// </summary>
        /// <param name="tags">Tag array</param>
        public Tags(IReadOnlyList<string> tags)
        {
            if (tags.Count != 2)
            {
                throw new StubbleException("Invalid Tags");
            }

            StartTag = tags[0];
            EndTag = tags[1];
        }

        /// <summary>
        /// Gets the start tag value
        /// </summary>
        public string StartTag { get; }

        /// <summary>
        /// Gets the end tag value
        /// </summary>
        public string EndTag { get; }

        /// <summary>
        /// Compares if two tags are equal
        /// </summary>
        /// <param name="left">left tag</param>
        /// <param name="right">other</param>
        /// <returns>If the tags are equal</returns>
        public static bool operator ==(Tags left, Tags right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two tags and returns if they're not equal
        /// </summary>
        /// <param name="left">tag</param>
        /// <param name="right">other</param>
        /// <returns>If the tags are not equal</returns>
        public static bool operator !=(Tags left, Tags right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns a visual representation of the tags
        /// </summary>
        /// <returns>The start tag and end tag</returns>
        public override string ToString()
        {
            return StartTag + " " + EndTag;
        }

        /// <inheritdoc/>
        public bool Equals(Tags other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(StartTag, other.StartTag) && string.Equals(EndTag, other.EndTag);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Tags)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (StartTag.GetHashCode() * 397) ^ EndTag.GetHashCode();
            }
        }
    }
}
