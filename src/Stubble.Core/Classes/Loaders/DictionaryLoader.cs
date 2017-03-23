// <copyright file="DictionaryLoader.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes.Loaders
{
    /// <summary>
    /// An <see cref="IStubbleLoader"/> for mapping strings to templates
    /// </summary>
    public class DictionaryLoader : IStubbleLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryLoader"/> class.
        /// </summary>
        /// <param name="templates">The templates to cache</param>
        public DictionaryLoader(IDictionary<string, string> templates)
        {
            TemplateCache = new Dictionary<string, string>(templates);
        }

        /// <summary>
        /// Gets the template cache for the loader
        /// </summary>
        internal IDictionary<string, string> TemplateCache { get; }

        /// <summary>
        /// Loads the template from the dictionary cache
        /// </summary>
        /// <param name="name">The name of the template</param>
        /// <returns>The template or null if not found</returns>
        public string Load(string name)
        {
            return TemplateCache.ContainsKey(name) ? TemplateCache[name] : null;
        }
    }
}
