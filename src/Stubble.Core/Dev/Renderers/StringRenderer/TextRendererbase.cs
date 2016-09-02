// <copyright file="TextRendererBase.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using Stubble.Core.Classes;

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
            get
            {
                return writer;
            }

            set
            {
                writer = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Render a given token
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>The writer</returns>
        public override object Render(ParserOutput token)
        {
            Write(token);
            return Writer;
        }
    }
}
