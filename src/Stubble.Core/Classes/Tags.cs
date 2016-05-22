// <copyright file="Tags.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Classes.Exceptions;

namespace Stubble.Core.Classes
{
    /// <summary>
    /// Represents a set of tags used to identify tokens
    /// </summary>
    public class Tags
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
        /// Returns a visual representation of the tags
        /// </summary>
        /// <returns>The start tag and end tag</returns>
        public override string ToString()
        {
            return StartTag + " " + EndTag;
        }
    }
}
