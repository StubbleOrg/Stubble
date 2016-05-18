// <copyright file="StubbleBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Loaders;
using Stubble.Core.Interfaces;

namespace Stubble.Core
{
    /// <summary>
    /// Represents the primary builder class for creating <see cref="StubbleRenderer"/> instances
    /// </summary>
    public sealed class StubbleBuilder : IStubbleBuilder
    {
        /// <summary>
        /// Gets the Template Loader
        /// </summary>
        internal IStubbleLoader TemplateLoader = new StringLoader();

        /// <summary>
        /// Gets the partial Template Loader
        /// </summary>
        internal IStubbleLoader PartialTemplateLoader;

        /// <summary>
        /// Gets the internal map of Value Getters
        /// </summary>
        internal IDictionary<Type, Func<object, string, object>> ValueGetters { get; } =
            new Dictionary<Type, Func<object, string, object>>();

        /// <summary>
        /// Gets the internal map of Token Getters
        /// </summary>
        internal IDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters { get; } =
            new Dictionary<string, Func<string, Tags, ParserOutput>>();

        /// <summary>
        /// Gets the internal list of Truthy Checks
        /// </summary>
        internal List<Func<object, bool?>> TruthyChecks { get; } =
            new List<Func<object, bool?>>();

        /// <summary>
        /// Gets the internal map of Enumeration Converters
        /// </summary>
        internal IDictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; } =
            new Dictionary<Type, Func<object, IEnumerable>>();

        /// <summary>
        /// Gets the max recursion depth for rendering templates
        /// </summary>
        internal int MaxRecursionDepth { get; private set; } = 256;

        /// <summary>
        /// Builds a <see cref="StubbleRenderer"/> instance with the initalised settings
        /// </summary>
        /// <returns>Returns a <see cref="StubbleRenderer"/> with the initalised settings</returns>
        public StubbleRenderer Build()
        {
            var registry = new Registry(new RegistrySettings
            {
                ValueGetters = ValueGetters,
                TokenGetters = TokenGetters,
                TruthyChecks = TruthyChecks,
                TemplateLoader = TemplateLoader,
                PartialTemplateLoader = PartialTemplateLoader,
                MaxRecursionDepth = MaxRecursionDepth,
                EnumerationConverters = EnumerationConverters
            });
            return new StubbleRenderer(registry);
        }

        /// <summary>
        /// Adds a given value getter to the Value Getters
        /// </summary>
        /// <param name="valueGetter">A value getter</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder AddValueGetter(KeyValuePair<Type, Func<object, string, object>> valueGetter)
        {
            ValueGetters.Add(valueGetter);
            return this;
        }

        /// <summary>
        /// Adds a given type and value getter function to the Value Getters
        /// </summary>
        /// <param name="type">The type to add the value getter function for</param>
        /// <param name="valueGetter">A value getter function</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder AddValueGetter(Type type, Func<object, string, object> valueGetter)
        {
            return AddValueGetter(new KeyValuePair<Type, Func<object, string, object>>(type, valueGetter));
        }

        /// <summary>
        /// Adds a given type and token getter function to the Token Getters
        /// </summary>
        /// <param name="tokenType">The token type to add the token getter function for</param>
        /// <param name="tokenGetter">A token getter function</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder AddTokenGetter(string tokenType, Func<string, Tags, ParserOutput> tokenGetter)
        {
            return AddTokenGetter(new KeyValuePair<string, Func<string, Tags, ParserOutput>>(tokenType, tokenGetter));
        }

        /// <summary>
        /// Adds a given token getter to the Token Getters
        /// </summary>
        /// <param name="tokenGetter">A token getter</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder AddTokenGetter(KeyValuePair<string, Func<string, Tags, ParserOutput>> tokenGetter)
        {
            TokenGetters.Add(tokenGetter);
            return this;
        }

        /// <summary>
        /// Adds a enumeration conversion to the Enumeration Convertions
        /// </summary>
        /// <param name="enumerationConversion">An enumeration conversion</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder AddEnumerationConversion(KeyValuePair<Type, Func<object, IEnumerable>> enumerationConversion)
        {
            EnumerationConverters.Add(enumerationConversion);
            return this;
        }

        /// <summary>
        /// Adds a enumeration conversion to the Enumeration Convertions
        /// </summary>
        /// <param name="type">The type to add an enumeration conversion function for</param>
        /// <param name="enumerationConversion">An enumeration conversion</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder AddEnumerationConversion(Type type, Func<object, IEnumerable> enumerationConversion)
        {
            return AddEnumerationConversion(new KeyValuePair<Type, Func<object, IEnumerable>>(type, enumerationConversion));
        }

        /// <summary>
        /// Adds a truthy check
        /// </summary>
        /// <param name="truthyCheck">A truthy check</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder AddTruthyCheck(Func<object, bool?> truthyCheck)
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
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder AddToTemplateLoader(IStubbleLoader loader)
        {
            return CombineLoaders(ref TemplateLoader, loader);
        }

        /// <summary>
        /// Sets the Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder SetTemplateLoader(IStubbleLoader loader)
        {
            TemplateLoader = loader;
            return this;
        }

        /// <summary>
        /// Sets the Partial Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder SetPartialTemplateLoader(IStubbleLoader loader)
        {
            PartialTemplateLoader = loader;
            return this;
        }

        /// <summary>
        /// Adds a loader to the Partial Template Loader. If the Partial Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Partial Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Partial Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder AddToPartialTemplateLoader(IStubbleLoader loader)
        {
            return CombineLoaders(ref PartialTemplateLoader, loader);
        }

        /// <summary>
        /// Sets the Max Recursion Depth for recursive templates
        /// </summary>
        /// <param name="maxRecursionDepth">the max depth for the recursion</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder SetMaxRecursionDepth(int maxRecursionDepth)
        {
            MaxRecursionDepth = maxRecursionDepth;
            return this;
        }

        private IStubbleBuilder CombineLoaders(ref IStubbleLoader currentLoader, IStubbleLoader loader)
        {
            var compositeLoader = currentLoader as CompositeLoader;
            if (compositeLoader != null)
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
