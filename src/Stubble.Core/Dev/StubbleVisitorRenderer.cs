// <copyright file="StubbleVisitorRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.IO;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Dev.Parser;
using Stubble.Core.Dev.Renderers;
using Stubble.Core.Interfaces;
using Stubble.Core.Classes.Loaders;

namespace Stubble.Core.Dev
{
    /// <summary>
    /// A renderer which renders a string using visitors
    /// </summary>
    public class StubbleVisitorRenderer : IStubbleRenderer
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
        }

        /// <summary>
        /// Gets the core Registry instance for the Renderer
        /// </summary>
        internal Registry Registry { get; }

        /// <inheritdoc/>
        public string Render(string template, object view)
        {
            return Render(template, view, null, null);
        }

        /// <inheritdoc/>
        public string Render(string template, object view, RenderSettings settings)
        {
            return Render(template, view, null, settings);
        }

        /// <inheritdoc/>
        public string Render(string template, object view, IDictionary<string, string> partials)
        {
            return Render(template, view, partials, null);
        }

        /// <inheritdoc/>
        public string Render(string template, object view, IDictionary<string, string> partials, RenderSettings settings)
        {
            var loadedTemplate = Registry.TemplateLoader.Load(template);

            if (loadedTemplate == null)
            {
                throw new UnknownTemplateException("No template was found with the name '" + template + "'");
            }

            var document = MustacheParser.Parse(loadedTemplate);

            var textwriter = new StringWriter();
            var renderer = new StringRender(textwriter);

            var partialsLoader = Registry.PartialTemplateLoader;
            if (partials != null && partials.Keys.Count > 0)
            {
                partialsLoader = new CompositeLoader(new DictionaryLoader(partials), Registry.PartialTemplateLoader);
            }

            // TODO: Figure out Partials
            renderer.Render(document, new Context(view, Registry, partialsLoader, settings ?? Registry.RenderSettings));

            renderer.Writer.Flush();
            return ((StringWriter)renderer.Writer).ToString();
        }
    }
}
