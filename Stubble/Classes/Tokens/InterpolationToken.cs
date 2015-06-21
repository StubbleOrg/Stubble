using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes.Tokens
{
    public abstract class InterpolationToken : ParserOutput
    {
        internal object InterpolateLambdaValueIfPossible(object value, Context context)
        {
            var functionValue = value as Func<dynamic, object>;
            return functionValue != null ? ((Func<dynamic, object>)value).Invoke(context.View) : value;
        }
    }
}
