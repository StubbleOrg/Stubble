// <copyright file="CompilerSettings.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using Stubble.Compilation.Class;
using Stubble.Compilation.Contexts;
using Stubble.Core.Classes;
using Stubble.Core.Interfaces;
using Stubble.Core.Parser;
using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Renderers;
using Stubble.Core.Settings;

namespace Stubble.Compilation.Settings
{
    /// <summary>
    /// The settings used for the compilation renderer
    /// </summary>
    public class CompilerSettings : BaseSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilerSettings"/> class.
        /// </summary>
        /// <param name="valueGetters">The value getters</param>
        /// <param name="truthyChecks">The truthy checks</param>
        /// <param name="enumerationConverters">The enumeration converters</param>
        /// <param name="rendererPipeline">The renderer pipeline to use</param>
        /// <param name="templateLoader">The template loader</param>
        /// <param name="partialLoader">The partial loader</param>
        /// <param name="maxRecursionDepth">The max recursion depth</param>
        /// <param name="ignoreCaseOnLookup">Should case be ignored on lookup</param>
        /// <param name="parser">The mustache parser to use</param>
        /// <param name="defaultTags">The default tags to use during parsing</param>
        /// <param name="parserPipeline">The parser pipeline to use during parsing</param>
        /// <param name="compilationSettings">The default compilation settings for each render</param>
        public CompilerSettings(
            Dictionary<Type, DefaultSettings.ValueGetterDelegate> valueGetters,
            Dictionary<Type, List<LambdaExpression>> truthyChecks,
            Dictionary<Type, EnumerationConverter> enumerationConverters,
            TokenRendererPipeline<CompilerContext> rendererPipeline,
            IStubbleLoader templateLoader,
            IStubbleLoader partialLoader,
            uint maxRecursionDepth,
            bool ignoreCaseOnLookup,
            IMustacheParser parser,
            Tags defaultTags,
            ParserPipeline parserPipeline,
            CompilationSettings compilationSettings)
            : base(templateLoader, partialLoader, maxRecursionDepth, ignoreCaseOnLookup, parser, defaultTags, parserPipeline)
        {
            ValueGetters = valueGetters.ToImmutableDictionary();
            TruthyChecks = truthyChecks.ToImmutableDictionary(k => k.Key, v => v.Value.ToImmutableList());
            EnumerationConverters = enumerationConverters.ToImmutableDictionary();
            RendererPipeline = rendererPipeline;
            CompilationSettings = compilationSettings;
        }

        /// <summary>
        /// Gets a map of Types to Value getter functions
        /// </summary>
        public ImmutableDictionary<Type, DefaultSettings.ValueGetterDelegate> ValueGetters { get; }

        /// <summary>
        /// Gets a readonly list of TruthyChecks
        /// </summary>
        public ImmutableDictionary<Type, ImmutableList<LambdaExpression>> TruthyChecks { get; }

        /// <summary>
        /// Gets a map of Types to Enumeration convert functions
        /// </summary>
        public ImmutableDictionary<Type, EnumerationConverter> EnumerationConverters { get; }

        /// <summary>
        /// Gets the renderer pipeline to be used when rendering
        /// </summary>
        public TokenRendererPipeline<CompilerContext> RendererPipeline { get; }

        /// <summary>
        /// Gets the compilation settings for use when compiling
        /// </summary>
        public CompilationSettings CompilationSettings { get; }
    }
}
