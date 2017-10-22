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
using Stubble.Core.Parser;
using Stubble.Core.Renderers;
using Stubble.Core.Renderers.Interfaces;

namespace Stubble.Core.Settings
{
    /// <summary>
    /// A builder class for creating a <see cref="RendererSettings"/> instance
    /// </summary>
    public class RendererSettingsBuilder : BaseSettingsBuilder<RendererSettingsBuilder, RendererSettings>
    {
        /// <summary>
        /// Gets the token renderers to be used by the renderer
        /// </summary>
        public OrderedList<ITokenRenderer> TokenRenderers { get; internal set; }
            = new OrderedList<ITokenRenderer>(RendererSettingsDefaults.DefaultTokenRenderers());

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
        /// Gets or sets the RenderSettings
        /// </summary>
        internal RenderSettings RenderSettings { get; set; }

        /// <summary>
        /// Gets or sets a map of Types to Enumeration convert functions
        /// </summary>
        internal Dictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; set; }
            = new Dictionary<Type, Func<object, IEnumerable>>();

        /// <summary>
        /// Builds a RegistrySettings class with all the provided details
        /// </summary>
        /// <returns>The registry settings</returns>
        public override RendererSettings BuildSettings()
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
                DefaultTags ?? new Tags("{{", "}}"),
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
    }
}
