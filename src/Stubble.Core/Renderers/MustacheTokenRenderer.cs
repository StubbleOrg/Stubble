// <copyright file="MustacheTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Stubble.Core.Contexts;
using Stubble.Core.Renderers.Interfaces;
using Stubble.Core.Tokens;

namespace Stubble.Core.Renderers
{
    /// <summary>
    /// A base class representing a mustache tag renderer
    /// </summary>
    /// <typeparam name="TRenderer">The type of the renderer</typeparam>
    /// <typeparam name="TToken">The type of tag that it renders</typeparam>
    /// <typeparam name="TContext">The type of the context used during rendering</typeparam>
    public abstract class MustacheTokenRenderer<TRenderer, TToken, TContext> : ITokenRenderer<TContext>
        where TRenderer : RendererBase<TContext>
        where TToken : MustacheToken
        where TContext : BaseContext<TContext>
    {
        /// <inheritdoc/>
        public bool Accept(RendererBase<TContext> renderer, MustacheToken obj)
        {
            return obj is TToken;
        }

        /// <inheritdoc/>
        public virtual void Write(RendererBase<TContext> renderer, MustacheToken obj, TContext context)
        {
            var typedRenderer = (TRenderer)renderer;
            var typedObj = (TToken)obj;

            Write(typedRenderer, typedObj, context);
        }

        /// <inheritdoc/>
        public Task WriteAsync(RendererBase<TContext> renderer, MustacheToken obj, TContext context)
        {
            var typedRenderer = (TRenderer)renderer;
            var typedObj = (TToken)obj;

            return WriteAsync(typedRenderer, typedObj, context);
        }

        /// <summary>
        /// Write the tag using the given renderer
        /// </summary>
        /// <param name="renderer">The renderer to use</param>
        /// <param name="obj">The tag to write</param>
        /// <param name="context">The context to write the tag with</param>
        protected abstract void Write(TRenderer renderer, TToken obj, TContext context);

        /// <summary>
        /// Write the tag using the given renderer
        /// </summary>
        /// <param name="renderer">The renderer to use</param>
        /// <param name="obj">The tag to write</param>
        /// <param name="context">The context to write the tag with</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task WriteAsync(TRenderer renderer, TToken obj, TContext context);
    }
}
