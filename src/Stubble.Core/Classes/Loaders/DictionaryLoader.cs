// <copyright file="ArrayLoader.cs" company="Stubble Authors">
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
        internal IDictionary<string, string> TemplateCache { get; }

        public DictionaryLoader(IDictionary<string, string> templates)
        {
            TemplateCache = new Dictionary<string, string>(templates);
        }

        public string Load(string name)
        {
            return TemplateCache.ContainsKey(name) ? TemplateCache[name] : null;
        }
    }
}
