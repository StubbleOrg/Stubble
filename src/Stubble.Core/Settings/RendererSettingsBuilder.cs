// <copyright file="RendererSettingsBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Builders;
using Stubble.Core.Classes;
using Stubble.Core.Helpers;
using Stubble.Core.Imported;
using Stubble.Core.Interfaces;
using Stubble.Core.Loaders;
using Stubble.Core.Parser;
using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Renderers;
using Stubble.Core.Renderers.Interfaces;

namespace Stubble.Core.Settings
{
    /// <summary>
    /// A builder class for creating a <see cref="RendererSettings"/> instance
    /// </summary>
    public class RendererSettingsBuilder
    {
        private IStubbleLoader templateLoader = new StringLoader();

        private IStubbleLoader partialTemplateLoader;

        /// <summary>
        /// Gets the token renderers to be used by the renderer
        /// </summary>
        public OrderedList<ITokenRenderer> TokenRenderers { get; internal set; }
            = new OrderedList<ITokenRenderer>(RendererSettingsDefaults.DefaultTokenRenderers());

        /// <summary>
        /// Gets the Template Loader
        /// </summary>
        internal IStubbleLoader TemplateLoader => templateLoader;

        /// <summary>
        /// Gets the partial Template Loader
        /// </summary>
        internal IStubbleLoader PartialTemplateLoader => partialTemplateLoader;

        /// <summary>
        /// Gets or sets a map of Types to Value getter functions
        /// </summary>
        internal Dictionary<Type, Func<object, string, object>> ValueGetters { get; set; }
            = new Dictionary<Type, Func<object, string, object>>();

        /// <summary>
        /// Gets or sets a readonly list of TruthyChecks
        /// </summary>
        internal List<Func<object, bool?>> TruthyChecks { get; set; }
            = new List<Func<object, bool?>>();

        /// <summary>
        /// Gets or sets the MaxRecursionDepth
        /// </summary>
        internal uint? MaxRecursionDepth { get; set; }

        /// <summary>
        /// Gets or sets the RenderSettings
        /// </summary>
        internal RenderSettings RenderSettings { get; set; }

        /// <summary>
        /// Gets or sets a map of Types to Enumeration convert functions
        /// </summary>
        internal Dictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; set; }
            = new Dictionary<Type, Func<object, IEnumerable>>();

        /// <summary>
        /// Gets or sets a value indicating whether keys should be looked up with case sensitivity
        /// </summary>
        internal bool IgnoreCaseOnKeyLookup { get; set; }

        /// <summary>
        /// Gets or sets the mustache parser to use
        /// </summary>
        internal IMustacheParser Parser { get; set; }

        /// <summary>
        /// Gets or sets the mustache parser pipeline to use during parsing
        /// </summary>
        internal ParserPipeline ParserPipeline { get; set; }

        /// <summary>
        /// Gets or sets the default tags to use during parsing
        /// </summary>
        internal Classes.Tags DefaultTags { get; set; }

        /// <summary>
        /// Builds a RegistrySettings class with all the provided details
        /// </summary>
        /// <returns>The registry settings</returns>
        public RendererSettings BuildSettings()
        {
            var mergedGetters = RendererSettingsDefaults.DefaultValueGetters(IgnoreCaseOnKeyLookup).MergeLeft(ValueGetters);

            mergedGetters = mergedGetters
                .OrderBy(x => x.Key, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable())
                .ToDictionary(item => item.Key, item => item.Value);

            return new RendererSettings(
                mergedGetters,
                TruthyChecks,
                TemplateLoader,
                PartialTemplateLoader,
                MaxRecursionDepth.HasValue ? (MaxRecursionDepth.Value > 0 ? MaxRecursionDepth.Value : uint.MaxValue) : 256,
                RenderSettings ?? RenderSettings.GetDefaultRenderSettings(),
                EnumerationConverters,
                IgnoreCaseOnKeyLookup,
                Parser ?? new CachedMustacheParser(),
                new TokenRendererPipeline(TokenRenderers),
                DefaultTags ?? new Classes.Tags("{{", "}}"),
                ParserPipeline ?? new ParserPipelineBuilder().Build());
        }

        /// <summary>
        /// Adds a given type and value getter function to the Value Getters
        /// </summary>
        /// <param name="type">The type to add the value getter function for</param>
        /// <param name="valueGetter">A value getter function</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder AddValueGetter(Type type, Func<object, string, object> valueGetter)
        {
            ValueGetters.Add(type, valueGetter);
            return this;
        }

        /// <summary>
        /// Adds a enumeration conversion to the Enumeration Convertions
        /// </summary>
        /// <param name="type">The type to add an enumeration conversion function for</param>
        /// <param name="enumerationConversion">An enumeration conversion</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder AddEnumerationConversion(
            Type type,
            Func<object, IEnumerable> enumerationConversion)
        {
            EnumerationConverters.Add(type, enumerationConversion);
            return this;
        }

        /// <summary>
        /// Adds a truthy check
        /// </summary>
        /// <param name="truthyCheck">A truthy check</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder AddTruthyCheck(Func<object, bool?> truthyCheck)
        {
            TruthyChecks.Add(truthyCheck);
            return this;
        }

        /// <summary>
        /// Adds a loader to the Template Loader. If the Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Template Loader</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder AddToTemplateLoader(IStubbleLoader loader)
        {
            return CombineLoaders(ref templateLoader, loader.Clone());
        }

        /// <summary>
        /// Sets the Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Template Loader</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder SetTemplateLoader(IStubbleLoader loader)
        {
            templateLoader = loader.Clone();
            return this;
        }

        /// <summary>
        /// Adds a loader to the Partial Template Loader. If the Partial Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Partial Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Partial Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Partial Template Loader</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder AddToPartialTemplateLoader(IStubbleLoader loader)
        {
            return CombineLoaders(ref partialTemplateLoader, loader.Clone());
        }

        /// <summary>
        /// Sets the Partial Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Partial Template Loader</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder SetPartialTemplateLoader(IStubbleLoader loader)
        {
            partialTemplateLoader = loader.Clone();
            return this;
        }

        /// <summary>
        /// Sets the Max Recursion Depth for recursive templates. A value of zero will set to uint.MaxValue
        /// </summary>
        /// <param name="maxRecursionDepth">the max depth for the recursion</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder SetMaxRecursionDepth(uint maxRecursionDepth)
        {
            MaxRecursionDepth = maxRecursionDepth;
            return this;
        }

        /// <summary>
        /// Sets if the case should be ignored when looking up keys in the context
        /// </summary>
        /// <param name="ignoreCaseOnKeyLookup">if the case should be ignored on key lookup</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder SetIgnoreCaseOnKeyLookup(bool ignoreCaseOnKeyLookup)
        {
            IgnoreCaseOnKeyLookup = ignoreCaseOnKeyLookup;
            return this;
        }

        /// <summary>
        /// Sets the mustache parser to use for the renderer settings
        /// </summary>
        /// <param name="parser">The parser to use</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public RendererSettingsBuilder SetMustacheParser(IMustacheParser parser)
        {
            Parser = parser;
            return this;
        }

        /// <summary>
        /// Sets the default tags to be used during parsing
        /// </summary>
        /// <param name="tags">The tags</param>
        /// <returns>The IRendererSettingsBuilder for chaining</returns>
        public RendererSettingsBuilder SetDefaultTags(Tags tags)
        {
            DefaultTags = tags;
            return this;
        }

        /// <summary>
        /// Sets the parser pipeline to be used during parsing
        /// </summary>
        /// <param name="pipeline">The pipeline to use</param>
        /// <returns>The IRendererSettingsBuilder for chaining</returns>
        public RendererSettingsBuilder SetParserPipeline(ParserPipeline pipeline)
        {
            ParserPipeline = pipeline;
            return this;
        }

        private RendererSettingsBuilder CombineLoaders(ref IStubbleLoader currentLoader, IStubbleLoader loader)
        {
            if (currentLoader is CompositeLoader compositeLoader)
            {
                var composite = compositeLoader;
                composite.AddLoader(loader);
            }
            else
            {
                var composite = new CompositeLoader(currentLoader, loader);
                currentLoader = composite;
            }

            return this;
        }
    }
}
