// <copyright file="ParserPipeline.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Stubble.Core.Dev.Parser;

namespace Stubble.Core.Dev
{
    /// <summary>
    /// This is used to configure the inline and block parsers
    /// </summary>
    public class ParserPipeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserPipeline"/> class.
        /// </summary>
        /// <param name="inlineParsers">The inline parsers</param>
        /// <param name="blockParsers">The block parsers</param>
        internal ParserPipeline(List<Parser.Parser> inlineParsers, List<BlockParser> blockParsers)
        {
            InlineParsers = inlineParsers ?? throw new ArgumentNullException(nameof(inlineParsers));
            BlockParsers = blockParsers ?? throw new ArgumentNullException(nameof(blockParsers));
        }

        /// <summary>
        /// Gets a readonly list of inline parsers
        /// </summary>
        internal List<Parser.Parser> InlineParsers { get; }

        /// <summary>
        /// Gets a readonly list of block parsers
        /// </summary>
        internal List<Parser.BlockParser> BlockParsers { get; }
    }
}
