using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes.Loaders
{
    public sealed class CompositeLoader : IStubbleLoader
    {
        private readonly List<IStubbleLoader> _loaders;

        public CompositeLoader(params IStubbleLoader[] loaders)
        {
            _loaders = new List<IStubbleLoader>(loaders);
        }

        public CompositeLoader AddLoader(IStubbleLoader loader)
        {
            _loaders.Add(loader);
            return this;
        }

        public CompositeLoader AddLoaders(params IStubbleLoader[] loader)
        {
            _loaders.AddRange(loader);
            return this;
        }

        /// <summary>
        /// Loads a template with the given name.
        ///
        /// Returns null if the template is not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A Mustache Template</returns>
        public string Load(string name)
        {
            foreach (var loader in _loaders.AsEnumerable().Reverse())
            {
                var loadedTemplate = loader.Load(name);
                if (loadedTemplate != null) return loadedTemplate;
            }

            throw new UnknownTemplateException("No template was found with the name '" + name + "'");
        }
    }
}
