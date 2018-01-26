// <copyright file="CompilerSettingsBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Stubble.Compilation.Contexts;
using Stubble.Core.Builders;
using Stubble.Core.Helpers;
using Stubble.Core.Imported;
using Stubble.Core.Parser;
using Stubble.Core.Renderers;
using Stubble.Core.Renderers.Interfaces;
using Stubble.Core.Settings;
using Getter = System.Func<System.Type, System.Linq.Expressions.Expression, string, System.Linq.Expressions.Expression>;

namespace Stubble.Compilation.Settings
{
    /// <summary>
    /// A builder class for compilation settings
    /// </summary>
    public class CompilerSettingsBuilder : BaseSettingsBuilder<CompilerSettingsBuilder, CompilerSettings>
    {
        /// <summary>
        /// Gets or sets the ordered list of Token Renderers
        /// </summary>
        public OrderedList<ITokenRenderer<CompilerContext>> TokenRenderers { get; protected set; }
            = new OrderedList<ITokenRenderer<CompilerContext>>(DefaultSettings.DefaultTokenRenderers());

        /// <summary>
        /// Gets or sets a map of Types to Value getter functions
        /// </summary>
        protected internal Dictionary<Type, Getter> ValueGetters { get; set; }
            = new Dictionary<Type, Getter>();

        /// <summary>
        /// Gets or sets a readonly list of TruthyChecks
        /// </summary>
        protected internal Dictionary<Type, LambdaExpression> TruthyChecks { get; set; }
            = new Dictionary<Type, LambdaExpression>();

        /// <summary>
        /// Gets or sets a map of Types to Enumeration convert functions
        /// </summary>
        protected internal Dictionary<Type, Expression<Func<object, IEnumerable>>> EnumerationConverters { get; set; }
            = new Dictionary<Type, Expression<Func<object, IEnumerable>>>();

        /// <summary>
        /// Gets or sets the RenderSettings
        /// </summary>
        protected internal CompilationSettings CompilationSettings { get; set; }

        /// <summary>
        /// Sets the compilation settings
        /// </summary>
        /// <param name="settings">The settings to use</param>
        /// <returns>The builder instance for chaining</returns>
        public CompilerSettingsBuilder SetCompilationSettings(CompilationSettings settings)
        {
            CompilationSettings = settings;
            return this;
        }

        /// <summary>
        /// Builds an instance of the compilation settings from the builder
        /// </summary>
        /// <returns>The built compilation settings</returns>
        public override CompilerSettings BuildSettings()
        {
            var mergedGetters = DefaultSettings.DefaultValueGetters(IgnoreCaseOnKeyLookup).MergeLeft(ValueGetters);

            return new CompilerSettings(
                mergedGetters,
                TruthyChecks,
                EnumerationConverters,
                new TokenRendererPipeline<CompilerContext>(TokenRenderers),
                TemplateLoader,
                PartialTemplateLoader,
                MaxRecursionDepth.HasValue ? (MaxRecursionDepth.Value > 0 ? MaxRecursionDepth.Value : uint.MaxValue) : 256,
                IgnoreCaseOnKeyLookup,
                Parser ?? new CachedMustacheParser(),
                DefaultTags ?? new Core.Classes.Tags("{{", "}}"),
                ParserPipeline ?? new ParserPipelineBuilder().Build(),
                CompilationSettings ?? CompilationSettings.GetDefaultRenderSettings());
        }

        /// <summary>
        /// Adds a truthy check for a specific type to determine if the value can be considered 'Truthy'
        /// </summary>
        /// <typeparam name="T">The type to evaluate</typeparam>
        /// <param name="expr">How to evaluate the value</param>
        /// <returns>The builder for chaining calls</returns>
        public CompilerSettingsBuilder AddTruthyCheck<T>(Expression<Func<T, bool>> expr)
        {
            TruthyChecks.Add(typeof(T), expr);

            return this;
        }
    }
}
