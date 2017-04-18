using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
{
    /// <summary>
    /// Represents a parser for a <see cref="MustacheTemplate"/>
    /// </summary>
    public interface IMustacheParser
    {
        /// <summary>
        /// Parses a string with the provided tags, indent and pipeline
        /// </summary>
        /// <param name="text">The string to parse</param>
        /// <param name="startingTags">The tags to start with</param>
        /// <param name="lineIndent">The indent to start at</param>
        /// <param name="pipeline">The pipeline to use</param>
        /// <returns>The parsed template</returns>
        MustacheTemplate Parse(
            string text,
            Classes.Tags startingTags = null,
            int lineIndent = 0,
            ParserPipeline pipeline = null);
    }
}
