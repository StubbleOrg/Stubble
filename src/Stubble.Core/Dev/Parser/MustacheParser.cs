// <copyright file="MustacheParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Stubble.Core.Dev.Imported;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
{
    /// <summary>
    /// A parser which takes a template and converts it into a list of Tags
    /// </summary>
    public class MustacheParser
    {
        private readonly Processor processor;
        private StringSlice content;

        private MustacheParser(string text, Classes.Tags startingTags, ParserPipeline pipeline)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            content = new StringSlice(text);

            processor = new Processor(pipeline.InlineParsers, pipeline.BlockParsers)
            {
                CurrentTags = startingTags
            };
        }

        /// <summary>
        /// Parse a template and return a <see cref="List{MustacheTag}"/>
        /// </summary>
        /// <param name="text">The text to be parsed</param>
        /// <param name="startingTags">The starting tag description</param>
        /// <param name="pipeline">The pipeline to use for parsing</param>
        /// <returns>The string converted to Tags</returns>
        public static List<MustacheTag> Parse(string text, Classes.Tags startingTags = null, ParserPipeline pipeline = null)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            startingTags = startingTags ?? new Classes.Tags("{{", "}}");
            pipeline = pipeline ?? new ParserPipelineBuilder().Build();

            var markdownParser = new MustacheParser(text, startingTags, pipeline);
            return markdownParser.Parse();
        }

        private List<MustacheTag> Parse()
        {
            processor.ProcessTemplate(content);

            return processor.Document;
        }
    }
}
