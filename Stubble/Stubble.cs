using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;

namespace Stubble.Core
{
    public class Stubble
    {
        public LimitedSizeConcurrentDictionary<string, IList<ParserOutput>> Cache { get; set; }

        public Stubble() : this(15)
        {
        }

        public Stubble(int cacheLimit)
        {
            Cache = new LimitedSizeConcurrentDictionary<string, IList<ParserOutput>>(cacheLimit);
        }

        public string Render(string template, object view)
        {
            throw new NotImplementedException();
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
    }
}
