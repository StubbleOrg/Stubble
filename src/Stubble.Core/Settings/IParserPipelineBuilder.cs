// <copyright file="IParserPipelineBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Parser.Interfaces;

namespace Stubble.Core.Settings
{
    /// <summary>
    /// Parse pipeline builder will be used to configure the parser pipeline.
    /// </summary>
    public interface IParserPipelineBuilder
    {
        /// <summary>
        /// Finds a parser with the provided type and replaces it with the new parser
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to replace the provided one with</param>
        /// <returns>The builder for chaining</returns>
        IParserPipelineBuilder Replace<T>(InlineParser parser)
            where T : InlineParser;

        /// <summary>
        /// Finds a parser with the provided type and replaces it with the new parser
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to replace the provided one with</param>
        /// <returns>The builder for chaining</returns>
        IParserPipelineBuilder Replace<T>(BlockParser parser)
           where T : BlockParser;

        /// <summary>
        /// Finds a parser with the provided type and adds the new parser after it
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to add after</param>
        /// <returns>The builder for chaining</returns>
        IParserPipelineBuilder AddAfter<T>(InlineParser parser)
            where T : InlineParser;

        /// <summary>
        /// Finds a parser with the provided type and adds the new parser after it
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to add after</param>
        /// <returns>The builder for chaining</returns>
        IParserPipelineBuilder AddAfter<T>(BlockParser parser)
            where T : BlockParser;

        /// <summary>
        /// Finds a parser with the provided type and adds the new parser before it
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to add before</param>
        /// <returns>The builder for chaining</returns>
        IParserPipelineBuilder AddBefore<T>(InlineParser parser)
            where T : InlineParser;

        /// <summary>
        /// Finds a parser with the provided type and adds the new parser before it
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to add before</param>
        /// <returns>The builder for chaining</returns>
        IParserPipelineBuilder AddBefore<T>(BlockParser parser)
            where T : BlockParser;

        /// <summary>
        /// Finds and remove a parser with the provided type
        /// </summary>
        /// <typeparam name="T">The type to remove</typeparam>
        /// <returns>The builder for chaining</returns>
        IParserPipelineBuilder Remove<T>();
    }
}
