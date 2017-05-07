// <copyright file="RendererBase.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Tokens;

namespace Stubble.Core.Renderers
{
    /// <summary>
    /// A base class representing a StubbleRenderer
    /// </summary>
    public abstract class RendererBase
    {
        private readonly TokenRendererPipeline rendererPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererBase"/> class.
        /// </summary>
        /// <param name="rendererPipeline">The renderer pipeline to use for rendering</param>
        protected RendererBase(TokenRendererPipeline rendererPipeline)
        {
            this.rendererPipeline = rendererPipeline;
        }

        /// <summary>
        /// Renders a given tag
        /// </summary>
        /// <param name="token">The tag to render</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>The current renderer</returns>
        public abstract object Render(MustacheToken token, Context context);

        /// <summary>
        /// Renders a block tag
        /// </summary>
        /// <param name="token">The block tag to render</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>The current renderer</returns>
        public abstract object Render(BlockToken token, Context context);

        /// <summary>
        /// Write the current tag to the renderer
        /// </summary>
        /// <typeparam name="T">The type of tag</typeparam>
        /// <param name="obj">The tag to write</param>
        /// <param name="context">The context to write the tag with</param>
        public void Write<T>(T obj, Context context)
            where T : MustacheToken
        {
            if (obj == null)
            {
                return;
            }

            var renderer = rendererPipeline.TryGetTokenRenderer(this, obj);

            renderer?.Write(this, obj, context);
        }
    }
}
