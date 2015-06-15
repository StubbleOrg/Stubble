using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Helpers
{
    internal static class StringExtensions
    {
        internal static string Slice(this string source, int start, int end)
        {
            if (end < 0)
            {
                end = source.Length + end;
            }
            var len = end - start;
            return source.Substring(start, len);
        }
    }
}
