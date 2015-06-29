using System.Collections.Generic;

namespace Stubble.Core.Classes.Tokens
{
    internal class UnescapedValueToken : InterpolationToken, IRenderableToken
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);
            value = InterpolateLambdaValueIfPossible(value, writer, context, partials);

            return value != null ? value.ToString() : null;
        }
    }
}
