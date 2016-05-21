using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Stubble.Core
{
    internal static class TypeHelper
    {
        public static bool IsInstanceOfType(this Type type, object instance)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().IsInstanceOfType(instance);
#else
            return type.IsInstanceOfType(instance);
#endif
        }

        public static bool IsAssignableFrom(this Type a, Type b)
        {
#if NETSTANDARD1_5
            return a.GetTypeInfo().IsAssignableFrom(b);
#else
            return a.IsAssignableFrom(b);
#endif
        }

        public static bool IsSubclassOf(this Type a, Type b)
        {
#if NETSTANDARD1_5
            return a.GetTypeInfo().IsSubclassOf(b);
#else
            return a.IsSubclassOf(b);
#endif
        }

        public static MemberInfo[] GetMember(this Type type, string name, BindingFlags bindingAttr)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().GetMember(name, bindingAttr);
#else
            return type.GetMember(name, bindingAttr);
#endif
        }
    }
}
