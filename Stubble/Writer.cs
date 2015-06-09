using System;
using System.Collections.Generic;
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

        public string Render(string template, object view, Dictionary<string, string> partials)
        {
            throw new NotImplementedException();
        }
    }
}
