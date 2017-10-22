// <copyright file="RendererSettings.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Stubble.Core.Interfaces;
using Stubble.Core.Parser;
using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Renderers;

namespace Stubble.Core.Settings
{
    /// <summary>
    /// Contains all of the immutable settings for the renderer
    /// </summary>
    public class RendererSettings : BaseSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RendererSettings"/> class.
        /// </summary>
        /// <param name="valueGetters">The value getters</param>
        /// <param name="truthyChecks">The truthy checks</param>
        /// <param name="templateLoader">The template loader</param>
        /// <param name="partialLoader">The partial loader</param>
        /// <param name="maxRecursionDepth">The max recursion depth</param>
        /// <param name="renderSettings">The render settings</param>
        /// <param name="enumerationConverters">The enumeration converters</param>
        /// <param name="ignoreCaseOnLookup">Should case be ignored on lookup</param>
        /// <param name="parser">The mustache parser to use</param>
        /// <param name="rendererPipeline">The renderer pipeline to use</param>
        /// <param name="defaultTags">The default tags to use during parsing</param>
        /// <param name="parserPipeline">The parser pipeline to use during parsing</param>
        public RendererSettings(
            Dictionary<Type, Func<object, string, object>> valueGetters,
            IEnumerable<Func<object, bool?>> truthyChecks,
            IStubbleLoader templateLoader,
            IStubbleLoader partialLoader,
            uint maxRecursionDepth,
            RenderSettings renderSettings,
            Dictionary<Type, Func<object, IEnumerable>> enumerationConverters,
            bool ignoreCaseOnLookup,
            IMustacheParser parser,
            TokenRendererPipeline rendererPipeline,
            Classes.Tags defaultTags,
            ParserPipeline parserPipeline)
            : base(
                  templateLoader,
                  partialLoader,
                  maxRecursionDepth,
                  ignoreCaseOnLookup,
                  parser,
                  defaultTags,
                  parserPipeline)
        {
            ValueGetters = valueGetters.ToImmutableDictionary();
            TruthyChecks = truthyChecks.ToImmutableArray();
            RenderSettings = renderSettings;
            EnumerationConverters = enumerationConverters.ToImmutableDictionary();
            RendererPipeline = rendererPipeline;
        }

        /// <summary>
        /// Gets a map of Types to Value getter functions
        /// </summary>
        public ImmutableDictionary<Type, Func<object, string, object>> ValueGetters { get; }

        /// <summary>
        /// Gets a readonly list of TruthyChecks
        /// </summary>
        public ImmutableArray<Func<object, bool?>> TruthyChecks { get; }

        /// <summary>
        /// Gets the RenderSettings
        /// </summary>
        public RenderSettings RenderSettings { get; }

        /// <summary>
        /// Gets a map of Types to Enumeration convert functions
        /// </summary>
        public ImmutableDictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; }

        /// <summary>
        /// Gets the renderer pipeline to be used when rendering
        /// </summary>
        public TokenRendererPipeline RendererPipeline { get; }
    }
}
