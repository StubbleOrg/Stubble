// <copyright file="ParserPipelineBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Parser;
using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Parser.TokenParsers;

namespace Stubble.Core.Builders
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
            InlineParsers = new List<InlineParser>
            {
                new CommentTagParser(),
                new DelimiterTagParser(),
                new PartialTagParser(),
                new InterpolationTagParser(),
            };

            BlockParsers = new List<BlockParser>
            {
                new SectionTagParser(),
                new InvertedSectionParser(),
            };
        }

        /// <summary>
        /// Gets the inline parsers
        /// </summary>
        public List<InlineParser> InlineParsers { get; }

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

        /// <summary>
        /// Finds a parser with the provided type and replaces it with the new parser
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to replace the provided one with</param>
        /// <returns>The builder for chaining</returns>
        public ParserPipelineBuilder Replace<T>(InlineParser parser)
            where T : InlineParser
        {
            Replace<T, InlineParser>(InlineParsers, parser);
            return this;
        }

        /// <summary>
        /// Finds a parser with the provided type and replaces it with the new parser
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to replace the provided one with</param>
        /// <returns>The builder for chaining</returns>
        public ParserPipelineBuilder Replace<T>(BlockParser parser)
            where T : BlockParser
        {
            Replace<T, BlockParser>(BlockParsers, parser);
            return this;
        }

        /// <summary>
        /// Finds a parser with the provided type and adds the new parser after it
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to add after</param>
        /// <returns>The builder for chaining</returns>
        public ParserPipelineBuilder AddAfter<T>(InlineParser parser)
            where T : InlineParser
        {
            AddAfter<T, InlineParser>(InlineParsers, parser);
            return this;
        }

        /// <summary>
        /// Finds a parser with the provided type and adds the new parser after it
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to add after</param>
        /// <returns>The builder for chaining</returns>
        public ParserPipelineBuilder AddAfter<T>(BlockParser parser)
            where T : BlockParser
        {
            AddAfter<T, BlockParser>(BlockParsers, parser);
            return this;
        }

        /// <summary>
        /// Finds a parser with the provided type and adds the new parser before it
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to add before</param>
        /// <returns>The builder for chaining</returns>
        public ParserPipelineBuilder AddBefore<T>(InlineParser parser)
            where T : InlineParser
        {
            AddBefore<T, InlineParser>(InlineParsers, parser);
            return this;
        }

        /// <summary>
        /// Finds a parser with the provided type and adds the new parser before it
        /// </summary>
        /// <typeparam name="T">The type to replace</typeparam>
        /// <param name="parser">The parser to add before</param>
        /// <returns>The builder for chaining</returns>
        public ParserPipelineBuilder AddBefore<T>(BlockParser parser)
            where T : BlockParser
        {
            AddBefore<T, BlockParser>(BlockParsers, parser);
            return this;
        }

        /// <summary>
        /// Finds and remove a parser with the provided type
        /// </summary>
        /// <typeparam name="T">The type to remove</typeparam>
        /// <returns>The builder for chaining</returns>
        public ParserPipelineBuilder Remove<T>()
        {
            if (typeof(InlineParser).IsAssignableFrom(typeof(T)))
            {
                Remove<T, InlineParser>(InlineParsers);
            }
            else if (typeof(BlockParser).IsAssignableFrom(typeof(T)))
            {
                Remove<T, BlockParser>(BlockParsers);
            }

            return this;
        }

        private static void Remove<T, TItem>(IList<TItem> collection)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var item = collection[i];
                if (item is T)
                {
                    collection.RemoveAt(i);
                    break;
                }
            }
        }

        private static void Replace<T, TItem>(IList<TItem> collection, TItem value)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var item = collection[i];
                if (item is T)
                {
                    collection[i] = value;
                }
            }
        }

        private static void AddAfter<T, TItem>(IList<TItem> collection, TItem value)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var item = collection[i];
                if (item is T)
                {
                    collection.Insert(i + 1, value);
                    break;
                }
            }
        }

        private static void AddBefore<T, TItem>(IList<TItem> collection, TItem value)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var item = collection[i];
                if (item is T)
                {
                    collection.Insert(i, value);
                    break;
                }
            }
        }
    }
}
