using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes.Loaders
{
    public sealed class CompositeLoader : IStubbleLoader
    {
        private readonly IStubbleLoader[] _loaders;

        public CompositeLoader(IStubbleLoader[] loaders)
        {
            _loaders = loaders;
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
            foreach (var loader in _loaders)
            {
                var loadedTemplate = loader.Load(name);
                if (loadedTemplate != null) return loadedTemplate;
            }

            throw new UnknownTemplateException("No template was found with the name '" + name + "'");
        }
    }
}
