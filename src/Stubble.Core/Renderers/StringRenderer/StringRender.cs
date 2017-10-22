// <copyright file="StringRender.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using Stubble.Core.Contexts;

namespace Stubble.Core.Renderers.StringRenderer
{
    /// <summary>
    /// A string renderer which renders tokens to strings
    /// </summary>
    public class StringRender : TextRendererBase<StringRender>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringRender"/> class
        /// with the provided text writer and the default depth
        /// </summary>
        /// <param name="writer">The writer to use</param>
        /// <param name="rendererPipeline">The renderer pipeline to use</param>
        public StringRender(TextWriter writer, TokenRendererPipeline<Context> rendererPipeline)
            : this(writer, rendererPipeline, 256)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringRender"/> class
        /// with the provided text writer and a given depth
        /// </summary>
        /// <param name="writer">The writer to use</param>
        /// <param name="rendererPipeline">The renderer pipeline to use</param>
        /// <param name="maxDepth">The max recursion depth for the renderer</param>
        public StringRender(TextWriter writer, TokenRendererPipeline<Context> rendererPipeline, uint maxDepth)
            : base(writer, rendererPipeline, maxDepth)
        {
        }
    }
}
