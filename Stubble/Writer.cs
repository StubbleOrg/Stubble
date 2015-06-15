using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;

namespace Stubble.Core
{
    public class Writer
    {
        public LimitedSizeConcurrentDictionary<string, IList<ParserOutput>> Cache { get; set; }

        public Writer(int cacheLimit)
        {
            Cache = new LimitedSizeConcurrentDictionary<string, IList<ParserOutput>>(cacheLimit);
        }

        public Writer()
            : this(15)
        {
        }

        public IList<ParserOutput> Parse(string template)
        {
            return Parse(template, Parser.DefaultTags);
        }

        public IList<ParserOutput> Parse(string template, Tags tags)
        {
            IList<ParserOutput> tokens;
            var success = Cache.TryGetValue(template, out tokens);
            if (!success)
            {
                tokens = Cache[template] = Parser.ParseTemplate(template, tags);
            }

            return tokens;
        }

        public string RenderTokens(IList<ParserOutput> tokens, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var sb = new StringBuilder();
            foreach (var token in tokens.OfType<IRenderableToken>( ))
            {
                var renderResult = token.Render(this, context, partials, originalTemplate);
                sb.Append(renderResult);
            }
            return sb.ToString();
        }

        public string Render(string template, object view, IDictionary<string, string> partials)
        {
            return Render(template, new Context(view), partials);
        }

        public string Render(string template, Context context, IDictionary<string, string> partials)
        {
            var tokens = Parse(template);
            var renderResult = RenderTokens(tokens, context, partials, template);
            return renderResult;
        }
    }
}
