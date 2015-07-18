using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes
{
    public struct TokenGetter
    {
        public string TokenType { get; set; }
        public Func<string, Tags, ParserOutput> Getter { get; set; }
    }
}
