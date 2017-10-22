// <copyright file="BaseSettings.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Interfaces;
using Stubble.Core.Parser;
using Stubble.Core.Parser.Interfaces;

namespace Stubble.Core.Settings
{
    /// <summary>
    /// Represents the root settings for a <see cref="IStubbleRenderer"/>
    /// </summary>
    public abstract class BaseSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSettings"/> class.
        /// </summary>
        /// <param name="templateLoader">The template loader</param>
        /// <param name="partialLoader">The partial loader</param>
        /// <param name="maxRecursionDepth">The max recursion depth</param>
        /// <param name="ignoreCaseOnLookup">Should case be ignored on lookup</param>
        /// <param name="parser">The mustache parser to use</param>
        /// <param name="defaultTags">The default tags to use during parsing</param>
        /// <param name="parserPipeline">The parser pipeline to use during parsing</param>
        public BaseSettings(
            IStubbleLoader templateLoader,
            IStubbleLoader partialLoader,
            uint maxRecursionDepth,
            bool ignoreCaseOnLookup,
            IMustacheParser parser,
            Classes.Tags defaultTags,
            ParserPipeline parserPipeline)
        {
            TemplateLoader = templateLoader;
            PartialTemplateLoader = partialLoader;
            MaxRecursionDepth = maxRecursionDepth;
            IgnoreCaseOnKeyLookup = ignoreCaseOnLookup;
            Parser = parser;
            DefaultTags = defaultTags;
            ParserPipeline = parserPipeline;
        }

        /// <summary>
        /// Gets the primary Template loader
        /// </summary>
        public IStubbleLoader TemplateLoader { get; }

        /// <summary>
        /// Gets the partial Template Loader
        /// </summary>
        public IStubbleLoader PartialTemplateLoader { get; }

        /// <summary>
        /// Gets the MaxRecursionDepth
        /// </summary>
        public uint MaxRecursionDepth { get; }

        /// <summary>
        /// Gets a value indicating whether keys should be looked up with case sensitivity
        /// </summary>
        public bool IgnoreCaseOnKeyLookup { get; }

        /// <summary>
        /// Gets the parser for mustache templates
        /// </summary>
        public IMustacheParser Parser { get; }

        /// <summary>
        /// Gets the parser pipeline to be used when parsing
        /// </summary>
        public ParserPipeline ParserPipeline { get; }

        /// <summary>
        /// Gets the default tags to be used during parsing
        /// </summary>
        public Classes.Tags DefaultTags { get; }
    }
}
