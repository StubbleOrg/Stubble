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
            var functionValueDynamic = value as Func<dynamic, object>;
            var functionValue = value as Func<object>;

            if(functionValueDynamic == null && functionValue == null) return value;

            object functionResult = functionValueDynamic != null ? functionValueDynamic.Invoke(context.View) : functionValue.Invoke();
            return writer.Render(functionResult.ToString(), context, partials);
        }
    }
}
