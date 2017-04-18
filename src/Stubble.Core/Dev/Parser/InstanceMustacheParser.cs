using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
{
    /// <summary>
    /// Represents wrapper to the static mustache parse call
    /// </summary>
    public class InstanceMustacheParser : IMustacheParser
    {
        /// <inheritdoc/>
        public MustacheTemplate Parse(string text, Classes.Tags startingTags = null, int lineIndent = 0, ParserPipeline pipeline = null)
        {
            return MustacheParser.Parse(text, startingTags, lineIndent, pipeline);
        }
    }
}
