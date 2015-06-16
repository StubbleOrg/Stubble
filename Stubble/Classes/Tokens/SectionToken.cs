using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Helpers;

namespace Stubble.Core.Classes.Tokens
{
    public class SectionToken : ParserOutput, IRenderableToken
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var buffer = new StringBuilder();
            var value = context.Lookup(Value);

            Func<string, string> subRender = (template) => writer.Render(template, context, partials);

            if (!ValueHelpers.IsTruthy(value)) return null;

            if (value is Array)
            {
                var arrayValue = value as Array;
                foreach (var v in arrayValue)
                {
                    buffer.Append(writer.RenderTokens(ChildTokens, context.Push(v), partials, originalTemplate));
                }
            }
            else if (value is Delegate)
            {
                if (originalTemplate == null) throw new Exception("Cannot use higher-order sections without the original template");
                var functionValue = value as Delegate;

                value = functionValue.DynamicInvoke(context.View, originalTemplate.Slice(Start, End), subRender);

                if (value != null)
                {
                    buffer.Append(value);
                }
            }
            else if (value is IDictionary || value != null)
            {
                buffer.Append(writer.RenderTokens(ChildTokens, context.Push(value), partials, originalTemplate));
            }
            else
            {
                buffer.Append(writer.RenderTokens(ChildTokens, context, partials, originalTemplate));
            }

            return buffer.ToString();
        }
    }
}
