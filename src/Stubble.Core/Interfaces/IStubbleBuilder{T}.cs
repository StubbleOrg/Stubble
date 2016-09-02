// <copyright file="IStubbleBuilder{T}.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Loaders;

namespace Stubble.Core.Interfaces
{
    /// <summary>
    /// Represents an interface for the stubble builder for initalizing <see cref="T:T"/> instances
    /// </summary>
    /// <typeparam name="T">The renderer instance this builder builds</typeparam>
    public interface IStubbleBuilder<out T> : IStubbleBuilder
    {
        /// <summary>
        /// Adds a given value getter to the Value Getters
        /// </summary>
        /// <param name="valueGetter">A value getter</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder<T> AddValueGetter(KeyValuePair<Type, Func<object, string, object>> valueGetter);

        /// <summary>
        /// Adds a given type and value getter function to the Value Getters
        /// </summary>
        /// <param name="type">The type to add the value getter function for</param>
        /// <param name="valueGetter">A value getter function</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> AddValueGetter(Type type, Func<object, string, object> valueGetter);

        /// <summary>
        /// Adds a given token getter to the Token Getters
        /// </summary>
        /// <param name="tokenGetter">A token getter</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> AddTokenGetter(KeyValuePair<string, Func<string, Tags, ParserOutput>> tokenGetter);

        /// <summary>
        /// Adds a given type and token getter function to the Token Getters
        /// </summary>
        /// <param name="tokenType">The token type to add the token getter function for</param>
        /// <param name="tokenGetter">A token getter function</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> AddTokenGetter(string tokenType, Func<string, Tags, ParserOutput> tokenGetter);

        /// <summary>
        /// Adds a enumeration conversion to the Enumeration Convertions
        /// </summary>
        /// <param name="enumerationConversion">An enumeration conversion</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> AddEnumerationConversion(KeyValuePair<Type, Func<object, IEnumerable>> enumerationConversion);

        /// <summary>
        /// Adds a enumeration conversion to the Enumeration Convertions
        /// </summary>
        /// <param name="type">The type to add an enumeration conversion function for</param>
        /// <param name="enumerationConversion">An enumeration conversion</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> AddEnumerationConversion(Type type, Func<object, IEnumerable> enumerationConversion);

        /// <summary>
        /// Adds a truthy check
        /// </summary>
        /// <param name="truthyCheck">A truthy check</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> AddTruthyCheck(Func<object, bool?> truthyCheck);

        /// <summary>
        /// Adds a loader to the Template Loader. If the Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Template Loader</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> AddToTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Sets the Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Template Loader</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> SetTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Adds a loader to the Partial Template Loader. If the Partial Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Partial Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Partial Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> AddToPartialTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Sets the Partial Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> SetPartialTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Sets the Max Recursion Depth for recursive templates
        /// </summary>
        /// <param name="maxRecursionDepth">the max depth for the recursion</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> SetMaxRecursionDepth(int maxRecursionDepth);

        /// <summary>
        /// Sets if the case should be ignored when looking up keys in the context
        /// </summary>
        /// <param name="ignoreCaseOnKeyLookup">if the case should be ignored on key lookup</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        IStubbleBuilder<T> SetIgnoreCaseOnKeyLookup(bool ignoreCaseOnKeyLookup);

        /// <summary>
        /// Builds a <see cref="StubbleStringRenderer"/> instance with the initalised settings
        /// </summary>
        /// <returns>Returns a <see cref="StubbleStringRenderer"/> with the initalised settings</returns>
        T Build();

        /// <summary>
        /// Converts the current builder into one that returns a different renderer
        /// </summary>
        /// <typeparam name="T1">The type of the new builder</typeparam>
        /// <returns>The existing builder returning the new type</returns>
        T1 SetBuilderType<T1>()
            where T1 : IStubbleBuilder, new();
    }
}
