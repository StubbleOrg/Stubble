// <copyright file="LiteralTagResult.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Parser
{
    /// <summary>
    /// A result of a literal tag match
    /// </summary>
    public enum LiteralTagResult
    {
        /// <summary>
        /// The match ended in a newline
        /// </summary>
        NewLine,

        /// <summary>
        /// The match ended at the end of file
        /// </summary>
        EndOfFile,

        /// <summary>
        /// The match ended at the start of another tag
        /// </summary>
        TagStart,

        /// <summary>
        /// The match ended and their was no content
        /// </summary>
        NoContent
    }
}