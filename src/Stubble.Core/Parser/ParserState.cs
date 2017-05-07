// <copyright file="ParserState.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Parser
{
    /// <summary>
    /// The result of a block parsing attempt
    /// </summary>
    public enum ParserState
    {
        /// <summary>
        /// No block parsed
        /// </summary>
        None,

        /// <summary>
        /// Skip the block
        /// </summary>
        Skip,

        /// <summary>
        /// Continue parsing the block
        /// </summary>
        Continue,

        /// <summary>
        /// Break the current line
        /// </summary>
        Break
    }
}
