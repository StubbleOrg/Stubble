// <copyright file="SectionEndTag.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Dev.Tags
{
    /// <summary>
    /// A block close tag representing the end of a section
    /// </summary>
    public class SectionEndTag : BlockCloseTag
    {
        /// <summary>
        /// Gets or sets the name of the section being closed
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the end position of the close tag
        /// </summary>
        public int EndPosition { get; set; }

        /// <summary>
        /// Gets or sets the end position for the content
        /// </summary>
        public int ContentEndPosition { get; set; }
    }
}
