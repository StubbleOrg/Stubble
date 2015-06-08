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
        private ConcurrentDictionary<string, IEnumerable<ParserOutput>> Cache { get; set; }

        public string Render(string template, object view)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ParserOutput> Parse(string template, Tags tags)
        {
            IEnumerable<ParserOutput> tokens;
            var success = Cache.TryGetValue(template, out tokens);
            if (!success)
                tokens = Cache[template] = Parser.ParseTemplate(template, tags);

            return tokens;
        }
    }
}
