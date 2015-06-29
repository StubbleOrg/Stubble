using System.Text.RegularExpressions;

namespace Stubble.Core
{
    public sealed class Scanner
    {
        public string Template { get; set; }
        public string Tail { get; set; }
        public int Pos { get; set; }
        public bool EOS { get { return string.IsNullOrEmpty(Tail); } }

        public Scanner(string template)
        {
            Template = template;
            Tail = template;
            Pos = 0;
        }

        public string Scan(Regex expression)
        {
            var matched = expression.Match(Tail);

            if (!matched.Success || matched.Index != 0)
            {
                return "";
            }

            var result = matched.Value;
            Tail = Tail.Substring(result.Length);
            Pos += result.Length;

            return result;
        }

        public string Scan(string expression)
        {
            return Scan(new Regex(expression));
        }

        public string ScanUntil(Regex expression)
        {
            var regexMatch = expression.Match(Tail);
            var index = regexMatch.Index;
            string match;

            if (!regexMatch.Success)
            {
                match = Tail;
                Tail = "";
            }
            else if (index == 0)
            {
                match = "";
            }
            else
            {
                match = Tail.Substring(0, index);
                Tail = Tail.Substring(index);
            }

            Pos += match.Length;
            return match;
        }

        public string ScanUntil(string expression)
        {
            return ScanUntil(new Regex(expression));
        }
    }
}
