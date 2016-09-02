// <copyright file="BlockParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Dev.Imported;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
{
    /// <summary>
    /// A Base class for all Block Parsers
    /// </summary>
    public abstract class BlockParser
    {
        /// <summary>
        /// Tries to open a block tag
        /// </summary>
        /// <param name="processor">The processor being used</param>
        /// <param name="slice">The string slice to parse</param>
        /// <returns>The result of trying to open the block</returns>
        public abstract ParserState TryOpenBlock(Processor processor, ref StringSlice slice);

        /// <summary>
        /// Tries to close a block tag
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="slice">The string slice to parse</param>
        /// <param name="tag">The tag to try and close</param>
        /// <returns>If the block was closed or not</returns>
        public abstract bool TryClose(Processor processor, ref StringSlice slice, BlockTag tag);

        /// <summary>
        /// Ends a block tag
        /// </summary>
        /// <param name="tag">The opening tag</param>
        /// <param name="closeTag">The closing tag</param>
        public abstract void EndBlock(BlockTag tag, BlockCloseTag closeTag);
    }
}
