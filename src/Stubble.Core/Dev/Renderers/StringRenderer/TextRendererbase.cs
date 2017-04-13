// <copyright file="TextRendererBase.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using Stubble.Core.Classes;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Renderers
{
    /// <summary>
    /// A base class for a text renderer
    /// </summary>
    public class TextRendererBase : RendererBase
    {
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRendererBase"/> class.
        /// </summary>
        /// <param name="writer">The text writer to use</param>
        protected TextRendererBase(TextWriter writer)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));

            // By default we output a newline with '\n' only even on Windows platforms
            Writer.NewLine = "\n";
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
            Write(token, context);
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
                Write(tag, context);
            }

            return Writer;
        }
    }
}
