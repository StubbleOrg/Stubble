// <copyright file="StringRender.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using Stubble.Core.Dev.Renderer;
using Stubble.Core.Dev.Renderers.StringRenderer;
using Stubble.Core.Dev.Renderers.Token;

namespace Stubble.Core.Dev.Renderers
{
    /// <summary>
    /// A string renderer which renders tokens to strings
    /// </summary>
    public class StringRender : TextRendererBase<StringRender>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringRender"/> class
        /// with the provided text writer
        /// </summary>
        /// <param name="writer">The writer to use</param>
        public StringRender(TextWriter writer)
            : base(writer)
        {
            Context = null;
            TokenRenderers.Add(new SectionTokenRenderer());
            TokenRenderers.Add(new RawValueTokenRenderer());
        }

        /// <summary>
        /// Gets the context for the renderer
        /// </summary>
        public Context Context { get; }
    }
}
