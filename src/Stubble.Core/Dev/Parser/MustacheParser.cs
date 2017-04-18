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

        private MustacheParser(string text, Classes.Tags startingTags, int lineIndent, ParserPipeline pipeline)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            content = new StringSlice(text);

            processor = new Processor(pipeline.InlineParsers, pipeline.BlockParsers)
            {
                CurrentTags = startingTags,
                LineIndent = lineIndent,
                DefaultLineIndent = lineIndent,
                DefaultLineIndentSlice = lineIndent > 0 ? new StringSlice(new string(' ', lineIndent)) : StringSlice.Empty
            };
        }

        /// <summary>
        /// Parse a template and return a <see cref="MustacheTemplate"/>
        /// </summary>
        /// <param name="text">The text to be parsed</param>
        /// <param name="startingTags">The starting tag description</param>
        /// <param name="lineIndent">The default line indent for the template</param>
        /// <param name="pipeline">The pipeline to use for parsing</param>
        /// <returns>The string converted to Tags</returns>
        public static MustacheTemplate Parse(string text, Classes.Tags startingTags = null, int lineIndent = 0, ParserPipeline pipeline = null)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            startingTags = startingTags ?? new Classes.Tags("{{", "}}");
            pipeline = pipeline ?? new ParserPipelineBuilder().Build();

            var mustacheParser = new MustacheParser(text, startingTags, lineIndent, pipeline);
            return mustacheParser.Parse();
        }

        private MustacheTemplate Parse()
        {
            processor.ProcessTemplate(content);

            return processor.Document;
        }
    }
}
