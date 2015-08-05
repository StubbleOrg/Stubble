using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Stubble.Core.Helpers;

namespace Stubble.Core.Classes.Tokens
{
    internal class SectionToken : ParserOutput, IRenderableToken, ISection
    {
        internal readonly static List<Type> EnumerableBlacklist = new List<Type>
        {
            typeof(IDictionary),
            typeof(string)
        };
        public Tags Tags { get; set; }

        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var buffer = new StringBuilder();
            var value = context.Lookup(Value);

            if (!context.IsTruthyValue(value)) return null;

            if (value is IEnumerable && !EnumerableBlacklist.Any(x => x.IsInstanceOfType(value)))
            {
                var arrayValue = value as IEnumerable;

                foreach (var v in arrayValue)
                {
                    buffer.Append(writer.RenderTokens(ChildTokens, context.Push(v), partials, originalTemplate));
                }
            }
            else if (value is Func<dynamic, string, object> || value is Func<string, object>)
            {
                var functionDynamicValue = value as Func<dynamic, string, object>;
                var functionStringValue = value as Func<string, object>;
                var sectionContent = originalTemplate.Slice(End, ParentSectionEnd);
                value = functionDynamicValue != null ? functionDynamicValue.Invoke(context.View, sectionContent) : functionStringValue.Invoke(sectionContent);
                value = writer.Render(value.ToString(), context, partials, Tags);

                if (value != null)
                {
                    buffer.Append(value);
                }
            }
            else if (value is IDictionary || value != null)
            {
                buffer.Append(writer.RenderTokens(ChildTokens, context.Push(value), partials, originalTemplate));
            }

            return buffer.ToString();
        }
    }
}
