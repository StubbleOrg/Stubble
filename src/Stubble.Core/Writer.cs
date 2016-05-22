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
        /// with a cacheLimit and explicit registry
        /// </summary>
        /// <param name="cacheLimit">The max size of the template token cache</param>
        /// <param name="registry">The registry to use</param>
        public Writer(int cacheLimit, Registry registry)
        {
            Registry = registry;
            Cache = new LimitedSizeConcurrentDictionary<string, IList<ParserOutput>>(cacheLimit);
            Parser = new Parser(Registry);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Writer"/> class
        /// with a explicit registry and cacheSize of 15
        /// </summary>
        /// <param name="registry">The registry to use</param>
        public Writer(Registry registry)
            : this(15, registry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Writer"/> class
        /// with a default <see cref="Registry"/> and a cache size of 15
        /// </summary>
        public Writer()
            : this(15, new Registry())
        {
        }

        /// <summary>
        /// Gets the Template Token Cache
        /// </summary>
        internal LimitedSizeConcurrentDictionary<string, IList<ParserOutput>> Cache { get; }

        /// <summary>
        /// Gets an internal Parser for parsing the template to tokens
        /// </summary>
        private Parser Parser { get; }

        /// <summary>
        /// Gets a copy of the registry
        /// </summary>
        private Registry Registry { get; }

        /// <summary>
        /// Gets or sets the current depth of the writer
        /// </summary>
        private int CurrentDepth { get; set; }

        /// <summary>
        /// Parses a template, looks up the template in the
        /// template token cache and returns cached version if possible.
        /// </summary>
        /// <param name="template">The template to parse</param>
        /// <returns>A list of tokens parsed from the template</returns>
        public IList<ParserOutput> Parse(string template) => Parse(template, null);

        /// <summary>
        /// Parses a template with the given tags, looks up the template in the
        /// template token cache and returns cached version if possible.
        /// </summary>
        /// <param name="template">The template to parse</param>
        /// <param name="tags">The tags to parse the template with</param>
        /// <returns>A list of tokens parsed from the template</returns>
        public IList<ParserOutput> Parse(string template, Tags tags)
        {
            IList<ParserOutput> tokens;
            var success = Cache.TryGetValue(template, out tokens);
            if (!success)
            {
                tokens = Cache[template] = Parser.ParseTemplate(template, tags);
            }

            return tokens;
        }

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
            var tokens = Parse(template, tags);
            var renderResult = RenderTokens(tokens, context, partials, template);
            ResetCurrentDepth();
            return renderResult;
        }

        /// <summary>
        /// Clears the Template Token Cache
        /// </summary>
        public void ClearCache() => Cache.Clear();

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
            foreach (var token in tokens.OfType<IRenderableToken>())
            {
                var renderResult = token.Render(this, context, partials, originalTemplate);
                sb.Append(renderResult);
            }

            return sb.ToString();
        }

        private void ResetCurrentDepth()
        {
            CurrentDepth = 0;
        }
    }
}
