// <copyright file="CachedMustacheParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Stubble.Core.Classes;
using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Tokens;

namespace Stubble.Core.Parser
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
            Cache = new LimitedSizeConcurrentDictionary<TemplateKey, MustacheTemplate>((int)cacheLimit);
        }

        /// <summary>
        /// Gets the Template Token Cache
        /// </summary>
        internal LimitedSizeConcurrentDictionary<TemplateKey, MustacheTemplate> Cache { get; }

        /// <inheritdoc/>
        public MustacheTemplate Parse(string text, Tags startingTags = null, int lineIndent = 0, ParserPipeline pipeline = null)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var key = new TemplateKey(text, startingTags, lineIndent);
            var success = Cache.TryGetValue(key, out MustacheTemplate template);
            if (!success)
            {
                template = Cache[key] = MustacheParser.Parse(text, startingTags, lineIndent, pipeline);
            }

            return template;
        }

        /// <summary>
        /// A composite key of template parameters for storage in the cached dictionary
        /// </summary>
        internal struct TemplateKey : IEquatable<TemplateKey>
        {
            private string template;
            private Tags startingTags;
            private int lineIndent;

            /// <summary>
            /// Initializes a new instance of the <see cref="TemplateKey"/> struct.
            /// </summary>
            /// <param name="template">The template for the key</param>
            /// <param name="startingTags">The starting tags for the key</param>
            /// <param name="lineIndent">The line indent for the key</param>
            public TemplateKey(string template, Tags startingTags, int lineIndent)
            {
                this.template = template;
                this.startingTags = startingTags;
                this.lineIndent = lineIndent;
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                int hash = 13;
                hash = (hash * 7) + template.GetHashCode();
                hash = (hash * 7) + startingTags?.GetHashCode() ?? 1;
                hash = (hash * 7) + lineIndent.GetHashCode();
                return hash;
            }

            /// <summary>
            /// Checks the equality of the object with the TemplateKey
            /// </summary>
            /// <param name="obj">The object to compare</param>
            /// <returns>If the object is equal to the template key</returns>
            public override bool Equals(object obj)
            {
                if (!(obj is TemplateKey))
                {
                    return false;
                }

                return Equals((TemplateKey)obj);
            }

            /// <summary>
            /// Checks the equality of one template key to another
            /// </summary>
            /// <param name="other">The other template key</param>
            /// <returns>If the other key is equal to this key</returns>
            public bool Equals(TemplateKey other)
            {
                return template.Equals(other.template) &&
                       ((startingTags == null && other.startingTags == null) || startingTags.Equals(other.startingTags)) &&
                       lineIndent.Equals(other.lineIndent);
            }
        }
    }
}
