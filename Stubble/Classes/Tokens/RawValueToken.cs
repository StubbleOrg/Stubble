using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes.Tokens
{
    public class RawValueToken : ParserOutput, IRenderableToken
    {
        public readonly StringBuilder ValueBuilder = new StringBuilder();

        public override string Value
        {
            get
            {
                return ValueBuilder.ToString();
            }
            set { ValueBuilder.Clear().Append(value); }
        }

        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            return Value;
        }
    }
}
