using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Stubble.Core
{
    internal static class TypeHelper
    {
        public static bool IsSubclassOf(this Type a, Type b)
        {
#if NETSTANDARD1_3
            return a.GetTypeInfo().IsSubclassOf(b);
#else
            return a.IsSubclassOf(b);
#endif
        }
    }
}
