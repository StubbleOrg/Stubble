namespace Stubble.Core.Interfaces
{
    public interface IStubbleLoader
    {
        /// <summary>
        /// Loads a template with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A Mustache Template</returns>
        string Load(string name);
    }
}
