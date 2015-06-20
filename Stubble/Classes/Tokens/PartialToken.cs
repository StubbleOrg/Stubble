using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes.Tokens
{
    public class PartialToken : ParserOutput, IRenderableToken
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            if (partials == null) return null;

            var value = partials.ContainsKey(Value) ? partials[Value] : null;
            return value != null ? writer.RenderTokens(writer.Parse(value), context, partials, value) : null;
        }
    }
}
