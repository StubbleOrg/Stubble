// <copyright file="StubbleVisitorRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Classes;
using OldParser = Stubble.Core.Parser;

namespace Stubble.Core.Dev
{
    /// <summary>
    /// A renderer which renders a string using visitors
    /// </summary>
    public class StubbleVisitorRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleVisitorRenderer"/> class
        /// with a default registry
        /// </summary>
        public StubbleVisitorRenderer()
            : this(new Registry())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleVisitorRenderer"/> class
        /// with the provided registry
        /// </summary>
        /// <param name="registry">The registry</param>
        public StubbleVisitorRenderer(Registry registry)
        {
            Registry = registry;
            Parser = new OldParser(Registry);
        }

        /// <summary>
        /// Gets the core Registry instance for the Renderer
        /// </summary>
        internal Registry Registry { get; }

        /// <summary>
        /// Gets the core Parser instance for the Renderer
        /// </summary>
        internal OldParser Parser { get; }

        /// <summary>
        /// Render the template with the provided data
        /// </summary>
        /// <param name="template">The template to render</param>
        /// <param name="view">The data to use</param>
        /// <returns>The rendered template</returns>
        public string Render(string template, object view)
        {
            return Render(template, view, null);
        }

        /// <summary>
        /// Render the template with the provided data and partials
        /// </summary>
        /// <param name="template">The template to render</param>
        /// <param name="view">The data to use</param>
        /// <param name="partials">The partials to use</param>
        /// <returns>The rendered template</returns>
        public string Render(string template, object view, IDictionary<string, string> partials)
        {
            return null;
        }
    }
}
