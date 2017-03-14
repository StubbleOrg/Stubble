// <copyright file="MustacheTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Renderers
{
    /// <summary>
    /// A base class representing a mustache tag renderer
    /// </summary>
    /// <typeparam name="TRenderer">The type of the renderer</typeparam>
    /// <typeparam name="TToken">The type of tag that it renders</typeparam>
    public abstract class MustacheTokenRenderer<TRenderer, TToken> : ITokenRenderer
        where TRenderer : RendererBase
        where TToken : MustacheTag
    {
        /// <inheritdoc/>
        public bool Accept(RendererBase renderer, MustacheTag obj)
        {
            return obj is TToken;
        }

        /// <inheritdoc/>
        public virtual void Write(RendererBase renderer, MustacheTag obj, Context context)
        {
            var typedRenderer = (TRenderer)renderer;
            var typedObj = (TToken)obj;

            Write(typedRenderer, typedObj, context);
        }

        /// <summary>
        /// Write the tag using the given renderer
        /// </summary>
        /// <param name="renderer">The renderer to use</param>
        /// <param name="obj">The tag to write</param>
        /// <param name="context">The context to write the tag with</param>
        protected abstract void Write(TRenderer renderer, TToken obj, Context context);
    }
}
