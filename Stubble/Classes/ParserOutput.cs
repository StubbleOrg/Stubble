using System.Collections.Generic;
using System.Linq;

namespace Stubble.Core.Classes
{
    public class ParserOutput
    {
        public string TokenType { get; set; }
        public virtual string Value { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public List<ParserOutput> ChildTokens { get; set; }
        public int ParentSectionEnd { get; set; }

        public override bool Equals(object obj)
        {
            var a = obj as ParserOutput;
            if (a == null) return false;

            if (!TokenType.Equals(a.TokenType)) return false;
            if (!Value.Equals(a.Value)) return false;
            if (!Start.Equals(a.Start)) return false;
            if (!End.Equals(a.End)) return false;
            if (!ParentSectionEnd.Equals(a.ParentSectionEnd)) return false;
            if (ChildTokens != null && a.ChildTokens != null)
            {
                if (ChildTokens.Count != a.ChildTokens.Count) return false;

                return !ChildTokens.Where((token, i) => !token.Equals(a.ChildTokens[i])).Any();
            }

            return !(ChildTokens == null & a.ChildTokens != null) && !(ChildTokens != null & a.ChildTokens == null);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) + TokenType.GetHashCode();
                hash = (13 * hash) + Value.GetHashCode();
                hash = (13 * hash) + Start.GetHashCode();
                hash = (13 * hash) + End.GetHashCode();
                hash = (13 * hash) + ParentSectionEnd.GetHashCode();
                return hash;
            }
        }
    }
}
