using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Exceptions;

namespace Stubble.Core
{
    public sealed class Writer
    {
        internal LimitedSizeConcurrentDictionary<string, IList<ParserOutput>> Cache { get; set; }
        internal Parser Parser;
        private readonly Registry Registry;
        private int currentDepth = 0;

        public Writer(int cacheLimit, Registry registry)
        {
            Registry = registry;
            Cache = new LimitedSizeConcurrentDictionary<string, IList<ParserOutput>>(cacheLimit);
            Parser = new Parser(Registry);
        }

        public Writer(Registry registry)
            : this(15, registry)
        {
        }

        public Writer()
            : this(15, new Registry()) { }

        public IList<ParserOutput> Parse(string template)
        {
            return Parse(template, null);
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
            currentDepth++;
            if(currentDepth >= Registry.MaxRecursionDepth)
                throw new StubbleException(string.Format("You have reached the maximum recursion limit of {0}.", Registry.MaxRecursionDepth));

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
            return Render(template, new Context(view, Registry), partials, null);
        }

        public string Render(string template, Context context, IDictionary<string, string> partials)
        {
            return Render(template, context, partials, null);
        }

        public string Render(string template, Context context, IDictionary<string, string> partials, Tags tags)
        {
            var tokens = Parse(template, tags);
            var renderResult = RenderTokens(tokens, context, partials, template);
            return renderResult;
        }

        public void ClearCache()
        {
            Cache.Clear();
        }
    }
}
