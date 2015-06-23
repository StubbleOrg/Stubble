using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;

namespace Stubble.Core
{
    public class Writer
    {
        public LimitedSizeConcurrentDictionary<string, IList<ParserOutput>> Cache { get; set; }
        internal Parser Parser;
        internal ReadOnlyDictionary<Type, Func<object, string, object>> ValueRegistry { get; set; }

        public Writer(int cacheLimit)
        {
            Cache = new LimitedSizeConcurrentDictionary<string, IList<ParserOutput>>(cacheLimit);
            Parser = new Parser();

            ValueRegistry = new ReadOnlyDictionary<Type, Func<object, string, object>>(new Dictionary<Type, Func<object, string, object>>
            {
                {
                    typeof (IDictionary<string, object>),
                    (value, key) =>
                    {
                        var castValue = value as IDictionary<string, object>;
                        return castValue != null ? castValue[key] : null;
                    }
                },
                {
                    typeof (IDictionary),
                    (value, key) =>
                    {
                        var castValue = value as IDictionary;
                        return castValue != null ? castValue[key] : null;
                    }
                },
                {
                    typeof (object), (value, key) =>
                    {
                        var type = value.GetType();
                        var propertyInfo = type.GetProperty(key);
                        return propertyInfo != null ? propertyInfo.GetValue(value, null) : null;
                    }
                }
            });
        }

        public Writer()
            : this(15)
        {
        }

        public IList<ParserOutput> Parse(string template)
        {
            return Parse(template, null);
        }

        public IList<ParserOutput> Parse(string template, Tags tags)
        {
            IList<ParserOutput> tokens;
            var success = Cache.TryGetValue(template, out tokens);
            if (!success)
            {
                tokens = Cache[template] = Parser.ParseTemplate(template, tags);
            }

            return tokens;
        }

        public string RenderTokens(IList<ParserOutput> tokens, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var sb = new StringBuilder();
            foreach (var token in tokens.OfType<IRenderableToken>( ))
            {
                var renderResult = token.Render(this, context, partials, originalTemplate);
                sb.Append(renderResult);
            }
            return sb.ToString();
        }

        public string Render(string template, object view, IDictionary<string, string> partials)
        {
            return Render(template, new Context(view, ValueRegistry), partials, null);
        }

        public string Render(string template, Context context, IDictionary<string, string> partials)
        {
            return Render(template, context, partials, null);
        }

        public string Render(string template, Context context, IDictionary<string, string> partials, Tags tags)
        {
            var tokens = Parse(template, tags);
            var renderResult = RenderTokens(tokens, context, partials, template);
            return renderResult;
        }
    }
}
