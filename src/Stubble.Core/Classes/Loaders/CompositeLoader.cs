// <copyright file="CompositeLoader.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes.Loaders
{
    /// <summary>
    /// An <see cref="IStubbleLoader"/> with child loaders
    /// </summary>
    public sealed class CompositeLoader : IStubbleLoader
    {
        private readonly List<IStubbleLoader> loaders;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeLoader"/> class
        /// with child loaders.
        /// </summary>
        /// <param name="loaders">A list of child loaders to initalise with</param>
        public CompositeLoader(params IStubbleLoader[] loaders)
        {
            this.loaders = new List<IStubbleLoader>(loaders.Where(i => i != null));
        }

        /// <summary>
        /// Adds a new loader to the composite loader.
        /// </summary>
        /// <param name="loader">The loader to add</param>
        /// <returns>The composite loader instance</returns>
        public CompositeLoader AddLoader(IStubbleLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }

            loaders.Add(loader);
            return this;
        }

        /// <summary>
        /// Adds multiple loaders to the composite loader.
        /// </summary>
        /// <param name="newLoaders">The loaders to add</param>
        /// <returns>The composite loader instance</returns>
        public CompositeLoader AddLoaders(params IStubbleLoader[] newLoaders)
        {
            loaders.AddRange(newLoaders.Where(i => i != null));
            return this;
        }

        /// <inheritdoc/>
        public IStubbleLoader Clone()
        {
            return new CompositeLoader(loaders.Select(i => i.Clone()).ToArray());
        }

        /// <summary>
        /// Loads a template with the given name.
        /// Returns null if the template is not found
        /// </summary>
        /// <param name="name">The name of the template to load</param>
        /// <exception cref="UnknownTemplateException">When a template is not found in the loader</exception>
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
