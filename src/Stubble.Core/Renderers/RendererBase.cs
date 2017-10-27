// <copyright file="RendererBase.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Stubble.Core.Contexts;
using Stubble.Core.Tokens;

namespace Stubble.Core.Renderers
{
    /// <summary>
    /// A base class representing a StubbleRenderer
    /// </summary>
    /// <typeparam name="TContext">The type of the context for the renderer</typeparam>
    public abstract class RendererBase<TContext>
        where TContext : BaseContext<TContext>
    {
        private readonly TokenRendererPipeline<TContext> rendererPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererBase{TContext}"/> class.
        /// </summary>
        /// <param name="rendererPipeline">The renderer pipeline to use for rendering</param>
        protected RendererBase(TokenRendererPipeline<TContext> rendererPipeline)
        {
            this.rendererPipeline = rendererPipeline;
        }

        /// <summary>
        /// Renders a given tag
        /// </summary>
        /// <param name="token">The tag to render</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>The current renderer</returns>
        public abstract object Render(MustacheToken token, TContext context);

        /// <summary>
        /// Renders a block tag
        /// </summary>
        /// <param name="token">The block tag to render</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>The current renderer</returns>
        public abstract object Render(BlockToken token, TContext context);

        /// <summary>
        /// Renders a given tag
        /// </summary>
        /// <param name="token">The tag to render</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>The current renderer</returns>
        public abstract ValueTask<object> RenderAsync(MustacheToken token, TContext context);

        /// <summary>
        /// Renders a block tag
        /// </summary>
        /// <param name="token">The block tag to render</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>The current renderer</returns>
        public abstract ValueTask<object> RenderAsync(BlockToken token, TContext context);

        /// <summary>
        /// Write the current tag to the renderer
        /// </summary>
        /// <typeparam name="T">The type of tag</typeparam>
        /// <param name="obj">The tag to write</param>
        /// <param name="context">The context to write the tag with</param>
        public void Write<T>(T obj, TContext context)
            where T : MustacheToken
        {
            if (obj == null)
            {
                return;
            }

            var renderer = rendererPipeline.TryGetTokenRenderer(this, obj);

            renderer?.Write(this, obj, context);
        }

        /// <summary>
        /// Write the current tag to the renderer
        /// </summary>
        /// <typeparam name="T">The type of tag</typeparam>
        /// <param name="obj">The tag to write</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task WriteAsync<T>(T obj, TContext context)
            where T : MustacheToken
        {
            if (obj == null)
            {
                return;
            }

            var renderer = rendererPipeline.TryGetTokenRenderer(this, obj);

            var task = renderer?.WriteAsync(this, obj, context);

            if (task != null)
            {
                await task;
            }
        }
    }
}
