using Stubble.Core.Dev.Imported;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Stubble.Core.Dev.Settings
{
    /// <summary>
    /// Contains the renderer settings defaults
    /// </summary>
    public static class RendererSettingsDefaults
    {
        private static readonly ConcurrentDictionary<Type, Tuple<Dictionary<string, Func<object, object>>,
            Dictionary<string, Func<object, object>>>> GettersCache
            =
            new ConcurrentDictionary<Type, Tuple<Dictionary<string, Func<object, object>>,
                Dictionary<string, Func<object, object>>>>();

        /// <summary>
        /// Returns the default value getters
        /// </summary>
        /// <param name="ignoreCase">If case should be ignored on lookup</param>
        /// <returns>The default value getters</returns>
        public static Dictionary<Type, Func<object, string, object>> DefaultValueGetters(
            bool ignoreCase)
        {
            return new Dictionary<Type, Func<object, string, object>>()
            {
                {
                    typeof(IList),
                    (value, key) =>
                    {
                        var castValue = value as IList;

                        int intVal;
                        if (int.TryParse(key, out intVal))
                        {
                            return castValue != null && intVal < castValue.Count ? castValue[intVal] : null;
                        }

                        return null;
                    }
                },
                {
                    typeof(IDictionary<string, object>),
                    (value, key) =>
                    {
                        var castValue = ignoreCase
                            ? new Dictionary<string, object>(
                                (IDictionary<string, object>)value,
                                StringComparer.OrdinalIgnoreCase)
                            : value as IDictionary<string, object>;

                        return castValue != null && castValue.ContainsKey(key) ? castValue[key] : null;
                    }
                },
                {
                    typeof(IDictionary),
                    (value, key) =>
                    {
                        var castValue = ignoreCase
                            ? new Dictionary<string, object>(
                                (IDictionary<string, object>)value,
                                StringComparer.OrdinalIgnoreCase)
                            : value as IDictionary;

                        return castValue?[key];
                    }
                },
                {
                    typeof(object), (value, key) => GetValueFromObjectByName(value, key, ignoreCase)
                }
            };
        }

        private static object GetValueFromObjectByName(object value, string key, bool ignoreCase)
        {
            var objectType = value.GetType();
            Tuple<Dictionary<string, Func<object, object>>, Dictionary<string, Func<object, object>>> typeLookup;
            if (!GettersCache.TryGetValue(objectType, out typeLookup))
            {
                var memberLookup = GetMemberLookup(objectType);
                var noCase =
                    new Dictionary<string, Func<object, object>>(memberLookup, StringComparer.OrdinalIgnoreCase);
                typeLookup = Tuple.Create(memberLookup, noCase);

                GettersCache.AddOrUpdate(objectType, typeLookup, (_, existing) => existing);
            }

            var lookup = ignoreCase ? typeLookup.Item2 : typeLookup.Item1;

            return lookup.ContainsKey(key) ? lookup[key](value) : null;
        }

        private static Dictionary<string, Func<object, object>> GetMemberLookup(Type objectType)
        {
            var members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                                BindingFlags.FlattenHierarchy);

            var dict = new Dictionary<string, Func<object, object>>(members.Length);
            var param = Expression.Parameter(typeof(object));
            var cast = Expression.Convert(param, objectType);

            foreach (var m in members)
            {
                Expression ex = null;
                switch (m)
                {
                    case FieldInfo fi:
                        ex = fi.IsStatic ? Expression.Field(null, fi) : Expression.Field(cast, fi);
                        break;
                    case PropertyInfo pi:
                        var getter = pi.GetGetMethod();
                        if (getter != null && IsZeroArityGetterMethod(getter))
                        {
                            ex = getter.IsStatic ? Expression.Call(getter) : Expression.Call(cast, getter);
                        }
                        else if (pi.GetIndexParameters().Length == 0)
                        {
                            ex = Expression.Property(cast, pi);
                        }

                        break;
                    case MethodInfo mi:
                        if (IsZeroArityGetterMethod(mi))
                        {
                            ex = mi.IsStatic ? Expression.Call(mi) : Expression.Call(cast, mi);
                        }

                        break;
                }

                if (ex == null)
                {
                    continue;
                }

                var func = Expression.Lambda<Func<object, object>>(Expression.Convert(ex, typeof(object)), param)
                    .Compile();

                dict.Add(m.Name, func);
            }

            return dict;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        private static bool IsZeroArityGetterMethod(MethodInfo mi)
        {
            return mi.GetParameters().Length == 0 && !mi.IsGenericMethod && mi.ReturnType != typeof(void);
        }
    }
}