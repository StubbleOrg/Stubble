using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes.Loaders
{
    public sealed class StringLoader : IStubbleLoader
    {
        /// <summary>
        /// Loads a template with the given name.
        ///
        /// Returns null if the template is not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A Mustache Template</returns>
        public string Load(string name)
        {
            return name;
        }
    }
}
