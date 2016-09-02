// <copyright file="StubbleBuilder{T}.cs" company="Stubble Authors">
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
    /// Represents an interface for the stubble builder for initalizing <see cref="T:T"/> instances
    /// </summary>
    /// <typeparam name="T">The renderer type to build</typeparam>
    public abstract class StubbleBuilder<T> : IStubbleBuilder<T>
    {
        private IStubbleLoader templateLoader = new StringLoader();

        private IStubbleLoader partialTemplateLoader;

        /// <summary>
        /// Gets the Template Loader
        /// </summary>
        internal IStubbleLoader TemplateLoader => templateLoader;

        /// <summary>
        /// Gets the partial Template Loader
        /// </summary>
        internal IStubbleLoader PartialTemplateLoader => partialTemplateLoader;

        /// <summary>
        /// Gets the internal map of Value Getters
        /// </summary>
        internal IDictionary<Type, Func<object, string, object>> ValueGetters { get; private set; } =
            new Dictionary<Type, Func<object, string, object>>();

        /// <summary>
        /// Gets the internal map of Token Getters
        /// </summary>
        internal IDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters { get; private set; } =
            new Dictionary<string, Func<string, Tags, ParserOutput>>();

        /// <summary>
        /// Gets the internal list of Truthy Checks
        /// </summary>
        internal List<Func<object, bool?>> TruthyChecks { get; private set; } =
            new List<Func<object, bool?>>();

        /// <summary>
        /// Gets the internal map of Enumeration Converters
        /// </summary>
        internal IDictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; private set; } =
            new Dictionary<Type, Func<object, IEnumerable>>();

        /// <summary>
        /// Gets the max recursion depth for rendering templates
        /// </summary>
        internal int MaxRecursionDepth { get; private set; } = 256;

        /// <summary>
        /// Gets a value indicating whether case should be ignored when looking up keys in the context
        /// </summary>
        internal bool IgnoreCaseOnKeyLookup { get; private set; }

        /// <summary>
        /// Builds a <see cref="T:T"/> instance with the initalised settings
        /// </summary>
        /// <returns>Returns a <see cref="T:T"/> with the initalised settings</returns>
        public abstract T Build();

        /// <summary>
        /// Converts the current builder into one that returns a different renderer
        /// </summary>
        /// <typeparam name="T1">The type of the new builder</typeparam>
        /// <returns>The existing builder returning the new type</returns>
        public T1 SetBuilderType<T1>()
            where T1 : IStubbleBuilder, new()
        {
            var builder = new T1();
            builder.FillFromRegistrySettings(BuildRegistrySettings());

            return builder;
        }

        /// <summary>
        /// Adds a given value getter to the Value Getters
        /// </summary>
        /// <param name="valueGetter">A value getter</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> AddValueGetter(KeyValuePair<Type, Func<object, string, object>> valueGetter)
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
        public IStubbleBuilder<T> AddValueGetter(Type type, Func<object, string, object> valueGetter)
        {
            return AddValueGetter(new KeyValuePair<Type, Func<object, string, object>>(type, valueGetter));
        }

        /// <summary>
        /// Adds a given type and token getter function to the Token Getters
        /// </summary>
        /// <param name="tokenType">The token type to add the token getter function for</param>
        /// <param name="tokenGetter">A token getter function</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> AddTokenGetter(string tokenType, Func<string, Tags, ParserOutput> tokenGetter)
        {
            return AddTokenGetter(new KeyValuePair<string, Func<string, Tags, ParserOutput>>(tokenType, tokenGetter));
        }

        /// <summary>
        /// Adds a given token getter to the Token Getters
        /// </summary>
        /// <param name="tokenGetter">A token getter</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> AddTokenGetter(KeyValuePair<string, Func<string, Tags, ParserOutput>> tokenGetter)
        {
            TokenGetters.Add(tokenGetter);
            return this;
        }

        /// <summary>
        /// Adds a enumeration conversion to the Enumeration Convertions
        /// </summary>
        /// <param name="enumerationConversion">An enumeration conversion</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> AddEnumerationConversion(KeyValuePair<Type, Func<object, IEnumerable>> enumerationConversion)
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
        public IStubbleBuilder<T> AddEnumerationConversion(Type type, Func<object, IEnumerable> enumerationConversion)
        {
            return AddEnumerationConversion(new KeyValuePair<Type, Func<object, IEnumerable>>(type, enumerationConversion));
        }

        /// <summary>
        /// Adds a truthy check
        /// </summary>
        /// <param name="truthyCheck">A truthy check</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> AddTruthyCheck(Func<object, bool?> truthyCheck)
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
        public IStubbleBuilder<T> AddToTemplateLoader(IStubbleLoader loader)
        {
            return CombineLoaders(ref templateLoader, loader);
        }

        /// <summary>
        /// Sets the Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> SetTemplateLoader(IStubbleLoader loader)
        {
            templateLoader = loader;
            return this;
        }

        /// <summary>
        /// Sets the Partial Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> SetPartialTemplateLoader(IStubbleLoader loader)
        {
            partialTemplateLoader = loader;
            return this;
        }

        /// <summary>
        /// Adds a loader to the Partial Template Loader. If the Partial Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Partial Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Partial Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> AddToPartialTemplateLoader(IStubbleLoader loader)
        {
            return CombineLoaders(ref partialTemplateLoader, loader);
        }

        /// <summary>
        /// Sets the Max Recursion Depth for recursive templates
        /// </summary>
        /// <param name="maxRecursionDepth">the max depth for the recursion</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> SetMaxRecursionDepth(int maxRecursionDepth)
        {
            MaxRecursionDepth = maxRecursionDepth;
            return this;
        }

        /// <summary>
        /// Sets if the case should be ignored when looking up keys in the context
        /// </summary>
        /// <param name="ignoreCaseOnKeyLookup">if the case should be ignored on key lookup</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public IStubbleBuilder<T> SetIgnoreCaseOnKeyLookup(bool ignoreCaseOnKeyLookup)
        {
            IgnoreCaseOnKeyLookup = ignoreCaseOnKeyLookup;
            return this;
        }

        /// <summary>
        /// Sets internal information from a <see cref="RegistrySettings"/> instance
        /// </summary>
        /// <param name="settings">The settings to fill using</param>
        public void FillFromRegistrySettings(RegistrySettings settings)
        {
            EnumerationConverters = settings.EnumerationConverters;
            IgnoreCaseOnKeyLookup = settings.IgnoreCaseOnKeyLookup;
            MaxRecursionDepth = settings.MaxRecursionDepth ?? MaxRecursionDepth;
            partialTemplateLoader = settings.PartialTemplateLoader;
            templateLoader = settings.TemplateLoader;
            TokenGetters = settings.TokenGetters;
            TruthyChecks = settings.TruthyChecks;
            ValueGetters = settings.ValueGetters;
        }

        /// <summary>
        /// Builds a Registry class from the internal members
        /// </summary>
        /// <returns>A registry based on the class contents</returns>
        internal RegistrySettings BuildRegistrySettings()
        {
            return new RegistrySettings
            {
                ValueGetters = ValueGetters,
                TokenGetters = TokenGetters,
                TruthyChecks = TruthyChecks,
                TemplateLoader = templateLoader,
                PartialTemplateLoader = partialTemplateLoader,
                MaxRecursionDepth = MaxRecursionDepth,
                EnumerationConverters = EnumerationConverters,
                IgnoreCaseOnKeyLookup = IgnoreCaseOnKeyLookup
            };
        }

        private IStubbleBuilder<T> CombineLoaders(ref IStubbleLoader currentLoader, IStubbleLoader loader)
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
