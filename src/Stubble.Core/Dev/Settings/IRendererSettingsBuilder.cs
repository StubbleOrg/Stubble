using System;
using System.Collections;
using Stubble.Core.Classes.Loaders;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Dev.Settings
{
    /// <summary>
    /// The interface for a builder for creating a <see cref="RendererSettings"/> instances
    /// </summary>
    /// <typeparam name="T">The type of the builder</typeparam>
    public interface IRendererSettingsBuilder<out T>
    {
        /// <summary>
        /// Adds a enumeration conversion to the Enumeration Convertions
        /// </summary>
        /// <param name="type">The type to add an enumeration conversion function for</param>
        /// <param name="enumerationConversion">An enumeration conversion</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        T AddEnumerationConversion(Type type, Func<object, IEnumerable> enumerationConversion);

        /// <summary>
        /// Adds a loader to the Partial Template Loader. If the Partial Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Partial Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Partial Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        T AddToPartialTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Adds a loader to the Template Loader. If the Template Loader is a <see cref="CompositeLoader"/> then
        /// the loader is added. If not then the Template Loader is updated with a <see cref="CompositeLoader"/>
        /// combining the Template Loader and loader parameter.
        /// </summary>
        /// <param name="loader">The loader to add to the Template Loader</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        T AddToTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Adds a truthy check
        /// </summary>
        /// <param name="truthyCheck">A truthy check</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        T AddTruthyCheck(Func<object, bool?> truthyCheck);

        /// <summary>
        /// Adds a given type and value getter function to the Value Getters
        /// </summary>
        /// <param name="type">The type to add the value getter function for</param>
        /// <param name="valueGetter">A value getter function</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        T AddValueGetter(Type type, Func<object, string, object> valueGetter);

        /// <summary>
        /// Builds a RegistrySettings class with all the provided details
        /// </summary>
        /// <returns>The registry settings that were built</returns>
        RendererSettings BuildSettings();

        /// <summary>
        /// Sets if the case should be ignored when looking up keys in the context
        /// </summary>
        /// <param name="ignoreCaseOnKeyLookup">if the case should be ignored on key lookup</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        T SetIgnoreCaseOnKeyLookup(bool ignoreCaseOnKeyLookup);

        /// <summary>
        /// Sets the Max Recursion Depth for recursive templates
        /// </summary>
        /// <param name="maxRecursionDepth">the max depth for the recursion</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        T SetMaxRecursionDepth(uint maxRecursionDepth);

        /// <summary>
        /// Sets the Partial Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Partial Template Loader</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        T SetPartialTemplateLoader(IStubbleLoader loader);

        /// <summary>
        /// Sets the Template Loader to be the passed loader
        /// </summary>
        /// <param name="loader">The loader to set as the Template Loader</param>
        /// <returns>The IStubbleBuilder{T} for chaining</returns>
        T SetTemplateLoader(IStubbleLoader loader);
    }
}