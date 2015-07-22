namespace Stubble.Core.Classes
{
    public class RenderSettings
    {
        public bool SkipRecursiveLookup { get; set; }
        public bool ThrowOnDataMiss { get; set; }

        public static RenderSettings GetDefaultRenderSettings()
        {
            return new RenderSettings
            {
                SkipRecursiveLookup = false,
                ThrowOnDataMiss = false
            };
        }
    }
}
