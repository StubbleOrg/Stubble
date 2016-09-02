// <copyright file="Writer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Exceptions;

namespace Stubble.Core
{
    /// <summary>
    /// Represents the Writer for Tokens and a wrapper over the Parser
    /// </summary>
    public sealed class Writer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Writer"/> class
        /// with explicit registry and parser instance
        /// </summary>
        /// <param name="registry">The registry to use</param>
        /// <param name="parser">A parser the can be used by the writer</param>
        public Writer(Registry registry, Parser parser)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            Registry = registry;
            Parser = parser;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Writer"/> class
        /// with a explicit registry and a default <see cref="Parser"/>
        /// </summary>
        /// <param name="registry">The registry to use</param>
        public Writer(Registry registry)
            : this(registry, new Parser(15, registry))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Writer"/> class
        /// with a default <see cref="Registry"/> and a default <see cref="Parser"/>
        /// </summary>
        public Writer()
            : this(new Registry())
        {
        }

        /// <summary>
        /// Gets a copy of the registry
        /// </summary>
        private Registry Registry { get; }

        /// <summary>
        /// Gets the internal parser instance to be used for interpolation
        /// </summary>
        private Parser Parser { get; }

        /// <summary>
        /// Gets or sets the current depth of the writer
        /// </summary>
        private int CurrentDepth { get; set; }

        /// <summary>
        /// Takes a template, view object, partials and render settings and Renders them
        /// using default tags
        /// </summary>
        /// <param name="template">The template to parse and render</param>
        /// <param name="view">The view object to use to render</param>
        /// <param name="partials">The partials available to the template</param>
        /// <param name="settings">The settings to use for rendering</param>
        /// <returns>The template rendered with the view object</returns>
        public string Render(string template, object view, IDictionary<string, string> partials, RenderSettings settings)
            => Render(template, new Context(view, Registry, settings), partials, null);

        /// <summary>
        /// Takes a template, context and partials and Renders them using default tags
        /// </summary>
        /// <param name="template">The template to parse and render</param>
        /// <param name="context">The context object to use</param>
        /// <param name="partials">The partials available to the template</param>
        /// <returns>The template rendered with the passed context object</returns>
        public string Render(string template, Context context, IDictionary<string, string> partials)
            => Render(template, context, partials, null);

        /// <summary>
        /// Takes a template, context, partials and tags and renders the template
        /// </summary>
        /// <param name="template">The template to parse and render</param>
        /// <param name="context">The context object to use to render</param>
        /// <param name="partials">The partials available to the template</param>
        /// <param name="tags">The tags to initalise the parser with</param>
        /// <returns>The template rendered with the passed context object</returns>
        public string Render(string template, Context context, IDictionary<string, string> partials, Tags tags)
        {
            var tokens = Parser.Parse(template, tags);
            return Render(template, tokens, context, partials);
        }

        /// <summary>
        /// Renders the preparsed tokens with the given context and partials.
        /// </summary>
        /// <param name="template">The template that the tokens were parsed from</param>
        /// <param name="tokens">The template as parsed tokens</param>
        /// <param name="context">The context object to use to render</param>
        /// <param name="partials">The partials available to the template</param>
        /// <returns>The tokens rendered with the passed context object</returns>
        public string Render(string template, IList<ParserOutput> tokens, Context context, IDictionary<string, string> partials)
        {
            var renderResult = RenderTokens(tokens, context, partials, template);
            return renderResult;
        }

        /// <summary>
        /// Takes a template, context, partials and originalTemplate and renders the template
        /// </summary>
        /// <param name="template">The template to parse and render</param>
        /// <param name="context">The context object to use to render</param>
        /// <param name="partials">The partials available to the template</param>
        /// <param name="originalTemplate">The original template</param>
        /// <returns>The template rendered with the passed context object</returns>
        public string RenderWithOriginalTemplate(string template, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var tokens = Parser.Parse(template);
            var renderResult = RenderTokens(tokens, context, partials, originalTemplate);
            return renderResult;
        }

        /// <summary>
        /// Renders a list of tokens with the context, partials and passed original template
        /// </summary>
        /// <param name="tokens">The tokens to render</param>
        /// <param name="context">The context to use when rendering</param>
        /// <param name="partials">The partials the are available</param>
        /// <param name="originalTemplate">The original template</param>
        /// <returns>The rendered tokens as a string</returns>
        internal string RenderTokens(IList<ParserOutput> tokens, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            CurrentDepth++;

            if (CurrentDepth >= Registry.MaxRecursionDepth)
            {
                throw new StubbleException(
                    $"You have reached the maximum recursion limit of {Registry.MaxRecursionDepth}.");
            }

            var sb = new StringBuilder();
            foreach (var token in tokens)
            {
                var renderable = token as IRenderableToken;
                if (renderable == null)
                {
                    continue;
                }

                var renderResult = renderable.Render(this, context, partials, originalTemplate);
                sb.Append(renderResult);
            }

            CurrentDepth--;
            return sb.ToString();
        }
    }
}
