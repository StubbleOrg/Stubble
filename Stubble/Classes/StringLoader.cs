using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes
{
    public class StringLoader : IStubbleLoader
    {
        public string Load(string name)
        {
            return name;
        }
    }
}
