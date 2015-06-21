using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes.Tokens
{
    public class UnescapedValueToken : InterpolationToken, IRenderableToken
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);
            value = InterpolateLambdaValueIfPossible(value, context);

            return value != null ? value.ToString() : null;
        }
    }
}
