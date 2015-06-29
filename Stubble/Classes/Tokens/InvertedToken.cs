using System.Collections.Generic;
using Stubble.Core.Helpers;

namespace Stubble.Core.Classes.Tokens
{
    internal class InvertedToken : InterpolationToken, IRenderableToken
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);
            value = InterpolateLambdaValueIfPossible(value, writer, context, partials);

            return !ValueHelpers.IsTruthy(value) ? writer.RenderTokens(ChildTokens, context, partials, originalTemplate) : null;
        }
    }
}
