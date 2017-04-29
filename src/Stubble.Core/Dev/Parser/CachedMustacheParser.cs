// <copyright file="CachedMustacheParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
{
    /// <summary>
    /// A mustache parser that caches the results with the same templates
    /// </summary>
    public class CachedMustacheParser : IMustacheParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedMustacheParser"/> class.
        /// </summary>
        public CachedMustacheParser()
            : this(15)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedMustacheParser"/> class.
        /// with a given cache limit
        /// </summary>
        /// <param name="cacheLimit">The cache limit</param>
        public CachedMustacheParser(uint cacheLimit)
        {
            Cache = new LimitedSizeConcurrentDictionary<string, MustacheTemplate>((int)cacheLimit);
        }

        /// <summary>
        /// Gets the Template Token Cache
        /// </summary>
        internal LimitedSizeConcurrentDictionary<string, MustacheTemplate> Cache { get; }

        /// <inheritdoc/>
        public MustacheTemplate Parse(string text, Classes.Tags startingTags = null, int lineIndent = 0, ParserPipeline pipeline = null)
        {
            var success = Cache.TryGetValue(text, out MustacheTemplate template);
            if (!success)
            {
                template = Cache[text] = MustacheParser.Parse(text, startingTags, lineIndent, pipeline);
            }

            return template;
        }
    }
}
