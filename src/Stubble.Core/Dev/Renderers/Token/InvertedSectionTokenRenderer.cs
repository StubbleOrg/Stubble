using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Renderers.Token
{
    /// <summary>
    /// A <see cref="StringObjectRenderer{InvertedSectionTag}"/> for rendering section tokens
    /// </summary>
    internal class InvertedSectionTokenRenderer : StringObjectRenderer<InvertedSectionTag>
    {
        /// <inheritdoc/>
        protected override void Write(StringRender renderer, InvertedSectionTag obj, Context context)
        {
            var value = context.Lookup(obj.SectionName);

            if (!context.IsTruthyValue(value))
            {
                renderer.Render(obj, context);
            }
        }
    }
}
