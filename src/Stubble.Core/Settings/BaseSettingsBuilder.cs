// <copyright file="BaseSettingsBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;
using Stubble.Core.Loaders;
using Stubble.Core.Parser;
using Stubble.Core.Parser.Interfaces;

namespace Stubble.Core.Settings
{
    /// <summary>
    /// A base class for creating setting instances
    /// </summary>
    /// <typeparam name="TBuilder">The builder instance to return</typeparam>
    /// <typeparam name="TSettings">The settings type the builder creates</typeparam>
    public abstract class BaseSettingsBuilder<TBuilder, TSettings>
        where TBuilder : BaseSettingsBuilder<TBuilder, TSettings>
    {
        private IStubbleLoader templateLoader = new StringLoader();

        private IStubbleLoader partialTemplateLoader;

        /// <summary>
        /// Gets the parser pipeline builder.
        /// </summary>
        protected internal ParserPipelineBuilder ParserPipelineBuilder { get; } = new ParserPipelineBuilder();

        /// <summary>
        /// Gets the Template Loader
        /// </summary>
        protected internal IStubbleLoader TemplateLoader => templateLoader;

        /// <summary>
        /// Gets the partial Template Loader
        /// </summary>
        protected internal IStubbleLoader PartialTemplateLoader => partialTemplateLoader;

        /// <summary>
        /// Gets or sets the MaxRecursionDepth
        /// </summary>
        protected internal uint? MaxRecursionDepth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether keys should be looked up with case sensitivity
        /// </summary>
        protected internal bool IgnoreCaseOnKeyLookup { get; set; }

        /// <summary>
        /// Gets or sets the mustache parser to use
        /// </summary>
        protected internal IMustacheParser Parser { get; set; }

        /// <summary>
        /// Gets or sets the default tags to use during parsing
        /// </summary>
        protected internal Classes.Tags DefaultTags { get; set; }

        /// <summary>
        /// Gets or sets the types blacklisted from sections
        /// </summary>
        protected internal HashSet<Type> SectionBlacklistTypes { get; set; }

        /// <summary>
        /// Builds a RegistrySettings class with all the provided details
        /// </summary>
        /// <returns>The registry settings</returns>
        public abstract TSettings BuildSettings();

        /// <summary>
        /// Adds a loader to the Template Loader. If the Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Template Loader</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public TBuilder AddToTemplateLoader(IStubbleLoader loader)
        {
            return CombineLoaders(ref templateLoader, loader.Clone());
        }

        /// <summary>
        /// Sets the Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Template Loader</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public TBuilder SetTemplateLoader(IStubbleLoader loader)
        {
            templateLoader = loader.Clone();
            return (TBuilder)this;
        }

        /// <summary>
        /// Adds a loader to the Partial Template Loader. If the Partial Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Partial Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Partial Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Partial Template Loader</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public TBuilder AddToPartialTemplateLoader(IStubbleLoader loader)
        {
            return CombineLoaders(ref partialTemplateLoader, loader.Clone());
        }

        /// <summary>
        /// Configure the parser pipeline.
        /// </summary>
        /// <param name="builder">The parser pipeline builder.</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining.</returns>
        public TBuilder ConfigureParserPipeline(Action<ParserPipelineBuilder> builder)
        {
            builder?.Invoke(ParserPipelineBuilder);
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the Partial Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Partial Template Loader</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public TBuilder SetPartialTemplateLoader(IStubbleLoader loader)
        {
            partialTemplateLoader = loader.Clone();
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the Max Recursion Depth for recursive templates. A value of zero will set to uint.MaxValue
        /// </summary>
        /// <param name="maxRecursionDepth">the max depth for the recursion</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public TBuilder SetMaxRecursionDepth(uint maxRecursionDepth)
        {
            MaxRecursionDepth = maxRecursionDepth;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets if the case should be ignored when looking up keys in the context
        /// </summary>
        /// <param name="ignoreCaseOnKeyLookup">if the case should be ignored on key lookup</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public TBuilder SetIgnoreCaseOnKeyLookup(bool ignoreCaseOnKeyLookup)
        {
            IgnoreCaseOnKeyLookup = ignoreCaseOnKeyLookup;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the mustache parser to use for the renderer settings
        /// </summary>
        /// <param name="parser">The parser to use</param>
        /// <returns>The <see cref="RendererSettingsBuilder"/> for chaining</returns>
        public TBuilder SetMustacheParser(IMustacheParser parser)
        {
            Parser = parser;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the default tags to be used during parsing
        /// </summary>
        /// <param name="tags">The tags</param>
        /// <returns>The IRendererSettingsBuilder for chaining</returns>
        public TBuilder SetDefaultTags(Classes.Tags tags)
        {
            DefaultTags = tags;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the parser pipeline to be used during parsing
        /// </summary>
        /// <param name="pipeline">The pipeline to use</param>
        /// <returns>The IRendererSettingsBuilder for chaining</returns>
        [Obsolete("Use the ConfigureParserPipeline to modify the pipeline.", true)]
        public TBuilder SetParserPipeline(ParserPipeline pipeline)
        {
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the blacklisted types for sections
        /// </summary>
        /// <param name="types">The types to be blacklisted</param>
        /// <returns>The IRendererSettingsBuilder for chaining</returns>
        public TBuilder SetSectionBlacklistTypes(HashSet<Type> types)
        {
            SectionBlacklistTypes = types;
            return (TBuilder)this;
        }

        private TBuilder CombineLoaders(ref IStubbleLoader currentLoader, IStubbleLoader loader)
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

            return (TBuilder)this;
        }
    }
}
