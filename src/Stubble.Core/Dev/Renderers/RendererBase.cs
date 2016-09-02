// <copyright file="RendererBase.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Stubble.Core.Classes;

namespace Stubble.Core.Dev.Renderers
{
    /// <summary>
    /// A base class representing a StubbleRenderer
    /// </summary>
    public abstract class RendererBase
    {
        private readonly Dictionary<Type, ITokenRenderer> renderersPerType;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererBase"/> class.
        /// </summary>
        protected RendererBase()
        {
            TokenRenderers = new List<ITokenRenderer>();
            renderersPerType = new Dictionary<Type, ITokenRenderer>();
        }

        /// <summary>
        /// Gets the tag renderers
        /// </summary>
        public List<ITokenRenderer> TokenRenderers { get; }

        /// <summary>
        /// Renders a given tag
        /// </summary>
        /// <param name="token">The tag to render</param>
        /// <returns>The current renderer</returns>
        public abstract object Render(ParserOutput token);

        /// <summary>
        /// Write the current tag to the renderer
        /// </summary>
        /// <typeparam name="T">The type of tag</typeparam>
        /// <param name="obj">The tag to write</param>
        public void Write<T>(T obj)
            where T : ParserOutput
        {
            if (obj == null)
            {
                return;
            }

            var objectType = obj.GetType();

            ITokenRenderer renderer;
            if (!renderersPerType.TryGetValue(objectType, out renderer))
            {
                for (int i = 0; i < TokenRenderers.Count; i++)
                {
                    var testRenderer = TokenRenderers[i];
                    if (testRenderer.Accept(this, obj))
                    {
                        renderersPerType[objectType] = renderer = testRenderer;
                        break;
                    }
                }
            }

            renderer?.Write(this, obj);
        }
    }
}
