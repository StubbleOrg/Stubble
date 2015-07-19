using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes.Loaders
{
    public class StringLoader : IStubbleLoader
    {
        public string Load(string name)
        {
            return name;
        }
    }
}
