// <copyright file="InlineParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Imported;

namespace Stubble.Core.Parser.Interfaces
{
    /// <summary>
    /// A base class for all inline parsers
    /// </summary>
    public abstract class InlineParser
    {
        /// <summary>
        /// Try to match a tag from the current slice
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="slice">The slice</param>
        /// <returns>If the parser matched a tag</returns>
        public abstract bool Match(Processor processor, ref StringSlice slice);
    }
}