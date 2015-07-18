using System.Collections.Generic;
using System.Linq;

namespace Stubble.Core.Helpers
{
    internal static class Helpers
    {
        /// <summary>
        /// A way to merge IDictionaries together with the right most keys overriding the left keys.
        /// Found here: http://stackoverflow.com/questions/294138/merging-dictionaries-in-c-sharp
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="me"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        internal static IDictionary<TK, TV> MergeLeft<TK, TV>(this IDictionary<TK, TV> me, params IDictionary<TK, TV>[] others)
        {
            var newMap = new Dictionary<TK, TV>(me);
            foreach (var p in (new List<IDictionary<TK, TV>> { me }).Concat(others).SelectMany(src => src))
            {
                newMap[p.Key] = p.Value;
            }
            return newMap;
        }
    }
}
