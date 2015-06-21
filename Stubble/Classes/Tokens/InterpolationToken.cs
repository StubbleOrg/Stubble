using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes.Tokens
{
    public abstract class InterpolationToken : ParserOutput
    {
        internal object InterpolateLambdaValueIfPossible(object value, Writer writer, Context context, IDictionary<string, string> partials)
        {
            var functionValue = value as Func<dynamic, object>;
            if (functionValue == null) return value;

            var functionResult = functionValue.Invoke(context.View);
            return writer.Render(functionResult.ToString(), context, partials);
        }
    }
}
