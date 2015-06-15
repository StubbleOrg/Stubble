using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Helpers;

namespace Stubble.Core.Classes.Tokens
{
    public class InvertedToken : ParserOutput, IRenderableToken
    {
        public string Render(Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);

            return !ValueHelpers.IsTruthy(value) ? Writer.RenderTokens(ChildTokens, context, partials, originalTemplate) : null;
        }
    }
}
