// <copyright file="StubbleBuilder{T}.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using Stubble.Core.Classes;
using Stubble.Core.Imported;
using Stubble.Core.Interfaces;
using Stubble.Core.Parser;
using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Renderers.Interfaces;
using Stubble.Core.Settings;

namespace Stubble.Core.Builders
{
    /// <summary>
    /// Represents an interface for the stubble builder for initalizing <see cref="T:T"/> instances
    /// </summary>
    /// <typeparam name="T">The renderer type to build</typeparam>
    public abstract class StubbleBuilder<T> : IStubbleBuilder<T>
    {
        /// <inheritdoc/>
        public OrderedList<ITokenRenderer> TokenRenderers => SettingsBuilder.TokenRenderers;

        /// <summary>
        /// Gets or sets the settings builder for stubble builder
        /// </summary>
        internal RendererSettingsBuilder SettingsBuilder { get; set; } = new RendererSettingsBuilder();

        /// <inheritdoc/>
        public IStubbleBuilder<T> AddEnumerationConversion(Type type, Func<object, IEnumerable> enumerationConversion)
        {
            SettingsBuilder.AddEnumerationConversion(type, enumerationConversion);
            return this;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> AddToPartialTemplateLoader(IStubbleLoader loader)
        {
            SettingsBuilder.AddToPartialTemplateLoader(loader);
            return this;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> AddToTemplateLoader(IStubbleLoader loader)
        {
            SettingsBuilder.AddToTemplateLoader(loader);
            return this;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> AddTruthyCheck(Func<object, bool?> truthyCheck)
        {
            SettingsBuilder.AddTruthyCheck(truthyCheck);
            return this;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> AddValueGetter(Type type, Func<object, string, object> valueGetter)
        {
            SettingsBuilder.AddValueGetter(type, valueGetter);
            return this;
        }

        /// <summary>
        /// Builds a <see cref="T:T"/> instance with the initalised settings
        /// </summary>
        /// <returns>Returns a <see cref="T:T"/> with the initalised settings</returns>
        public abstract T Build();

        /// <inheritdoc/>
        public RendererSettings BuildSettings()
            => SettingsBuilder.BuildSettings();

        /// <summary>
        /// Converts the current builder into one that returns a different renderer
        /// </summary>
        /// <typeparam name="T1">The type of the new builder</typeparam>
        /// <returns>The existing builder returning the new type</returns>
        public T1 SetBuilderType<T1>()
            where T1 : IStubbleBuilder, new()
        {
            var builder = new T1();
            builder.SetRendererSettings(SettingsBuilder);

            return builder;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> SetMustacheParser(IMustacheParser parser)
        {
            SettingsBuilder.Parser = parser;
            return this;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> SetDefaultTags(Tags tags)
        {
            SettingsBuilder.DefaultTags = tags;
            return this;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> SetIgnoreCaseOnKeyLookup(bool ignoreCaseOnKeyLookup)
        {
            SettingsBuilder.SetIgnoreCaseOnKeyLookup(ignoreCaseOnKeyLookup);
            return this;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> SetMaxRecursionDepth(uint maxRecursionDepth)
        {
            SettingsBuilder.SetMaxRecursionDepth(maxRecursionDepth);
            return this;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> SetPartialTemplateLoader(IStubbleLoader loader)
        {
            SettingsBuilder.SetPartialTemplateLoader(loader);
            return this;
        }

        /// <inheritdoc/>
        public void SetRendererSettings(RendererSettingsBuilder settingsBuilder)
        {
            SettingsBuilder = settingsBuilder;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> SetTemplateLoader(IStubbleLoader loader)
        {
            SettingsBuilder.SetTemplateLoader(loader);
            return this;
        }

        /// <inheritdoc/>
        public IStubbleBuilder<T> SetParserPipeline(ParserPipeline pipeline)
        {
            SettingsBuilder.SetParserPipeline(pipeline);
            return this;
        }
    }
}
