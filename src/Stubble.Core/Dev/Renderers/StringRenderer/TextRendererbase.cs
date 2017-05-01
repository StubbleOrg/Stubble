// <copyright file="TextRendererBase.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Dev.Settings;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Renderers
{
    /// <summary>
    /// A base class for a text renderer
    /// </summary>
    public class TextRendererBase : RendererBase
    {
        private readonly uint maxDepth;
        private TextWriter writer;
        private uint currentDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRendererBase"/> class.
        /// </summary>
        /// <param name="writer">The text writer to use</param>
        /// <param name="rendererPipeline">The renderer pipeline to use</param>
        /// <param name="maxDepth">The max recursion depth for the renderer</param>
        protected TextRendererBase(TextWriter writer, TokenRendererPipeline rendererPipeline, uint maxDepth)
            : base(rendererPipeline)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));

            // By default we output a newline with '\n' only even on Windows platforms
            Writer.NewLine = "\n";
            this.maxDepth = maxDepth;
        }

        /// <summary>
        /// Gets or sets the text writer
        /// </summary>
        public TextWriter Writer
        {
            get => writer;

            set => writer = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Render a given token
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>The writer</returns>
        public override object Render(MustacheTag token, Context context)
        {
            currentDepth++;
            if (currentDepth >= maxDepth)
            {
                throw new StubbleException(
                    $"You have reached the maximum recursion limit of {maxDepth}.");
            }

            Write(token, context);
            currentDepth--;
            return Writer;
        }

        /// <summary>
        /// Renders a block tag and its children
        /// </summary>
        /// <param name="block">The tag to render</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>The writer</returns>
        public override object Render(BlockTag block, Context context)
        {
            foreach (var tag in block.Children)
            {
                currentDepth++;
                if (currentDepth >= maxDepth)
                {
                    throw new StubbleException(
                        $"You have reached the maximum recursion limit of {maxDepth}.");
                }

                Write(tag, context);
                currentDepth--;
            }

            return Writer;
        }
    }
}
