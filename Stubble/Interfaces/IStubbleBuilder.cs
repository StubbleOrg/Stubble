// <copyright file="IStubbleBuilder.cs" company="Stubble Authors">
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
    /// Represents an interface for the stubble builder for initalizing <see cref="StubbleRenderer"/> instances
    /// </summary>
    public interface IStubbleBuilder
    {
        /// <summary>
        /// Adds a given value getter to the Value Getters
        /// </summary>
        /// <param name="valueGetter">A value getter</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder AddValueGetter(KeyValuePair<Type, Func<object, string, object>> valueGetter);

        /// <summary>
        /// Adds a given type and value getter function to the Value Getters
        /// </summary>
        /// <param name="type">The type to add the value getter function for</param>
        /// <param name="valueGetter">A value getter function</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder AddValueGetter(Type type, Func<object, string, object> valueGetter);

        /// <summary>
        /// Adds a given token getter to the Token Getters
        /// </summary>
        /// <param name="tokenGetter">A token getter</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder AddTokenGetter(KeyValuePair<string, Func<string, Tags, ParserOutput>> tokenGetter);

        /// <summary>
        /// Adds a given type and token getter function to the Token Getters
        /// </summary>
        /// <param name="tokenType">The token type to add the token getter function for</param>
        /// <param name="tokenGetter">A token getter function</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder AddTokenGetter(string tokenType, Func<string, Tags, ParserOutput> tokenGetter);

        /// <summary>
        /// Adds a enumeration conversion to the Enumeration Convertions
        /// </summary>
        /// <param name="enumerationConversion">An enumeration conversion</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder AddEnumerationConversion(KeyValuePair<Type, Func<object, IEnumerable>> enumerationConversion);

        /// <summary>
        /// Adds a enumeration conversion to the Enumeration Convertions
        /// </summary>
        /// <param name="type">The type to add an enumeration conversion function for</param>
        /// <param name="enumerationConversion">An enumeration conversion</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder AddEnumerationConversion(Type type, Func<object, IEnumerable> enumerationConversion);

        /// <summary>
        /// Adds a truthy check
        /// </summary>
        /// <param name="truthyCheck">A truthy check</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder AddTruthyCheck(Func<object, bool?> truthyCheck);

        /// <summary>
        /// Adds a loader to the Template Loader. If the Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder AddToTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Sets the Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder SetTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Adds a loader to the Partial Template Loader. If the Partial Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Partial Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Partial Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder AddToPartialTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Sets the Partial Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder SetPartialTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Sets the Max Recursion Depth for recursive templates
        /// </summary>
        /// <param name="maxRecursionDepth">the max depth for the recursion</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder SetMaxRecursionDepth(int maxRecursionDepth);

        /// <summary>
        /// Sets if the case should be ignored when looking up keys in the context
        /// </summary>
        /// <param name="ignoreCaseOnKeyLookup">if the case should be ignored on key lookup</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        IStubbleBuilder SetIgnoreCaseOnKeyLookup(bool ignoreCaseOnKeyLookup);

        /// <summary>
        /// Builds a <see cref="StubbleRenderer"/> instance with the initalised settings
        /// </summary>
        /// <returns>Returns a <see cref="StubbleRenderer"/> with the initalised settings</returns>
        StubbleRenderer Build();
    }
}
