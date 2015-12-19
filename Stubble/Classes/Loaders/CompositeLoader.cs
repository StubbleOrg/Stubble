// <copyright file="CompositeLoader.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes.Loaders
{
    public sealed class CompositeLoader : IStubbleLoader
    {
        private readonly List<IStubbleLoader> loaders;

        public CompositeLoader(params IStubbleLoader[] loaders)
        {
            this.loaders = new List<IStubbleLoader>(loaders);
        }

        public CompositeLoader AddLoader(IStubbleLoader loader)
        {
            loaders.Add(loader);
            return this;
        }

        public CompositeLoader AddLoaders(params IStubbleLoader[] loader)
        {
            loaders.AddRange(loader);
            return this;
        }

        /// <summary>
        /// Loads a template with the given name.
        ///
        /// Returns null if the template is not found
        /// </summary>
        /// <param name="name">The name of the template to load</param>
        /// <returns>A Mustache Template</returns>
        public string Load(string name)
        {
            foreach (var loader in loaders.AsEnumerable().Reverse())
            {
                var loadedTemplate = loader.Load(name);
                if (loadedTemplate != null)
                {
                    return loadedTemplate;
                }
            }

            throw new UnknownTemplateException("No template was found with the name '" + name + "'");
        }
    }
}
