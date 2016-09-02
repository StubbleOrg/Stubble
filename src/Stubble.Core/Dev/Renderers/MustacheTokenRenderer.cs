// <copyright file="MustacheTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;

namespace Stubble.Core.Dev.Renderers
{
    /// <summary>
    /// A base class representing a mustache tag renderer
    /// </summary>
    /// <typeparam name="TRenderer">The type of the renderer</typeparam>
    /// <typeparam name="TToken">The type of tag that it renders</typeparam>
    public abstract class MustacheTokenRenderer<TRenderer, TToken> : ITokenRenderer
        where TRenderer : RendererBase
        where TToken : ParserOutput
    {
        /// <inheritdoc/>
        public bool Accept(RendererBase renderer, ParserOutput obj)
        {
            return obj is TToken;
        }

        /// <inheritdoc/>
        public virtual void Write(RendererBase renderer, ParserOutput obj)
        {
            var typedRenderer = (TRenderer)renderer;
            var typedObj = (TToken)obj;

            Write(typedRenderer, typedObj);
        }

        /// <summary>
        /// Write the tag using the given renderer
        /// </summary>
        /// <param name="renderer">The renderer to use</param>
        /// <param name="obj">The tag to write</param>
        protected abstract void Write(TRenderer renderer, TToken obj);
    }
}
