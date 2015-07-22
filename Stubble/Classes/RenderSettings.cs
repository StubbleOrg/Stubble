namespace Stubble.Core.Classes
{
    public class RenderSettings
    {
        public bool RecursiveLookup { get; set; }
        public bool ThrowOnDataMiss { get; set; }

        public static RenderSettings GetDefaultRenderSettings()
        {
            return new RenderSettings
            {
                RecursiveLookup = true,
                ThrowOnDataMiss = false
            };
        }
    }
}
