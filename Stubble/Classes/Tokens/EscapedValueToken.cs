using System.Collections.Generic;
using System.Net;

namespace Stubble.Core.Classes.Tokens
{
    internal class EscapedValueToken : InterpolationToken, IRenderableToken
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);
            value = InterpolateLambdaValueIfPossible(value, writer, context, partials);

            return value != null ? WebUtility.HtmlEncode(value.ToString()) : null;
        }
    }
}
