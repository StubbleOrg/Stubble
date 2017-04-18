using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Stubble.Core.Classes;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Dev.Settings
{
    /// <summary>
    /// Contains all of the immutable settings for the renderer
    /// </summary>
    public class RendererSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RendererSettings"/> class.
        /// </summary>
        /// <param name="valueGetters">The value getters</param>
        /// <param name="truthyChecks">The truthy checks</param>
        /// <param name="templateLoader">The template loader</param>
        /// <param name="partialLoader">The partial loader</param>
        /// <param name="maxRecursionDepth">The max recursion depth</param>
        /// <param name="renderSettings">The render settings</param>
        /// <param name="enumerationConverters">The enumeration converters</param>
        /// <param name="ignoreCaseOnLookup">Should case be ignored on lookup</param>
        public RendererSettings(
            Dictionary<Type, Func<object, string, object>> valueGetters,
            IEnumerable<Func<object, bool?>> truthyChecks,
            IStubbleLoader templateLoader,
            IStubbleLoader partialLoader,
            int maxRecursionDepth,
            RenderSettings renderSettings,
            Dictionary<Type, Func<object, IEnumerable>> enumerationConverters,
            bool ignoreCaseOnLookup)
        {
            ValueGetters = valueGetters.ToImmutableDictionary();
            TruthyChecks = truthyChecks.ToImmutableArray();
            TemplateLoader = templateLoader;
            PartialTemplateLoader = partialLoader;
            MaxRecursionDepth = maxRecursionDepth;
            RenderSettings = renderSettings;
            EnumerationConverters = enumerationConverters.ToImmutableDictionary();
            IgnoreCaseOnKeyLookup = ignoreCaseOnLookup;
        }

        /// <summary>
        /// Gets a map of Types to Value getter functions
        /// </summary>
        public ImmutableDictionary<Type, Func<object, string, object>> ValueGetters { get; }

        /// <summary>
        /// Gets a readonly list of TruthyChecks
        /// </summary>
        public ImmutableArray<Func<object, bool?>> TruthyChecks { get; }

        /// <summary>
        /// Gets the primary Template loader
        /// </summary>
        public IStubbleLoader TemplateLoader { get; }

        /// <summary>
        /// Gets the partial Template Loader
        /// </summary>
        public IStubbleLoader PartialTemplateLoader { get; }

        /// <summary>
        /// Gets the MaxRecursionDepth
        /// </summary>
        public int MaxRecursionDepth { get; }

        /// <summary>
        /// Gets the RenderSettings
        /// </summary>
        public RenderSettings RenderSettings { get; }

        /// <summary>
        /// Gets a map of Types to Enumeration convert functions
        /// </summary>
        public ImmutableDictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; }

        /// <summary>
        /// Gets a value indicating whether keys should be looked up with case sensitivity
        /// </summary>
        public bool IgnoreCaseOnKeyLookup { get; }
    }
}
