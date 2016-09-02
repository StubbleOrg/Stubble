// <copyright file="ParserPipelineBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Dev.Parser;

namespace Stubble.Core.Dev
{
    /// <summary>
    /// This class allows modification of the parser pipeline for use in
    /// parsing a Mustache template
    /// </summary>
    public class ParserPipelineBuilder
    {
        private ParserPipeline pipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserPipelineBuilder"/> class.
        /// </summary>
        public ParserPipelineBuilder()
        {
            InlineParsers = new List<Parser.Parser>
            {
                new CommentTagParser(),
                new DelimiterTagParser(),
                new PartialTagParser(),
                new InterpolationTagParser(),
            };

            BlockParsers = new List<BlockParser>
            {
                new SectionTagParser(),
            };
        }

        /// <summary>
        /// Gets the inline parsers
        /// </summary>
        public List<Parser.Parser> InlineParsers { get; }

        /// <summary>
        /// Gets the block parsers
        /// </summary>
        public List<BlockParser> BlockParsers { get; }

        /// <summary>
        /// Builds a pipeline instance and caches it so once it's built it
        /// can't be modified.
        /// </summary>
        /// <returns>A pipeline for use in parsing</returns>
        public ParserPipeline Build()
        {
            if (pipeline != null)
            {
                return pipeline;
            }

            pipeline = new ParserPipeline(InlineParsers, BlockParsers);
            return pipeline;
        }
    }
}
