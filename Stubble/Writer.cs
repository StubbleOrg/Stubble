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
    public sealed class Writer
    {
        internal LimitedSizeConcurrentDictionary<string, IList<ParserOutput>> Cache { get; set; }

        internal Parser Parser;
        private readonly Registry registry;
        private int currentDepth = 0;

        public Writer(int cacheLimit, Registry registry)
        {
            this.registry = registry;
            Cache = new LimitedSizeConcurrentDictionary<string, IList<ParserOutput>>(cacheLimit);
            Parser = new Parser(this.registry);
        }

        public Writer(Registry registry)
            : this(15, registry)
        {
        }

        public Writer()
            : this(15, new Registry())
        {
        }

        public IList<ParserOutput> Parse(string template)
        {
            return Parse(template, null);
        }

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

        public string RenderTokens(IList<ParserOutput> tokens, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            currentDepth++;

            if (currentDepth >= registry.MaxRecursionDepth)
            {
                throw new StubbleException(string.Format("You have reached the maximum recursion limit of {0}.", registry.MaxRecursionDepth));
            }

            var sb = new StringBuilder();
            foreach (var token in tokens.OfType<IRenderableToken>())
            {
                var renderResult = token.Render(this, context, partials, originalTemplate);
                sb.Append(renderResult);
            }

            return sb.ToString();
        }

        public string Render(string template, object view, IDictionary<string, string> partials, RenderSettings settings)
        {
            return Render(template, new Context(view, registry, settings), partials, null);
        }

        public string Render(string template, Context context, IDictionary<string, string> partials)
        {
            return Render(template, context, partials, null);
        }

        public string Render(string template, Context context, IDictionary<string, string> partials, Tags tags)
        {
            var tokens = Parse(template, tags);
            var renderResult = RenderTokens(tokens, context, partials, template);
            ResetCurrentDepth();
            return renderResult;
        }

        public void ClearCache()
        {
            Cache.Clear();
        }

        public void ResetCurrentDepth()
        {
            currentDepth = 0;
        }
    }
}
