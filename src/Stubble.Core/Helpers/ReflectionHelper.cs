// <copyright file="ReflectionHelper.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Stubble.Core.Imported;

namespace Stubble.Core.Helpers
{
    /// <summary>
    /// A collection of helpers for reflecting upon types
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Returns a lookup of a types members returning lazy Func accessors
        /// </summary>
        /// <param name="objectType">The type to lookup members from</param>
        /// <returns>A lookup of membername to lazy accessor func</returns>
        public static Dictionary<string, Lazy<Func<object, object>>> GetMemberFunctionLookup(Type objectType)
        {
            var members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                                BindingFlags.FlattenHierarchy);

            var dict = new Dictionary<string, Lazy<Func<object, object>>>(members.Length);
            var param = Expression.Parameter(typeof(object));
            var cast = Expression.Convert(param, objectType);

            foreach (var m in members)
            {
                var ex = GetExpressionFromMemberInfo(m, cast);

                if (ex == null)
                {
                    continue;
                }

                var func = new Lazy<Func<object, object>>(() => Expression
                    .Lambda<Func<object, object>>(Expression.Convert(ex, typeof(object)), param)
                    .Compile());

                dict.Add(m.Name, func);
            }

            return dict;
        }

        /// <summary>
        /// Gets Expression to call a <see cref="MemberInfo"/>
        /// </summary>
        /// <param name="m">The member info to call</param>
        /// <param name="instance">The instance to call the member on</param>
        /// <returns>An expression to call the member info using</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static Expression GetExpressionFromMemberInfo(MemberInfo m, Expression instance)
        {
            Expression ex = null;
            switch (m)
            {
                case FieldInfo fi:
                    ex = fi.IsStatic ? Expression.Field(null, fi) : Expression.Field(instance, fi);
                    break;
                case PropertyInfo pi:
                    var getter = pi.GetGetMethod();
                    if (getter != null && IsZeroArityGetterMethod(getter))
                    {
                        ex = getter.IsStatic ? Expression.Call(getter) : Expression.Call(instance, getter);
                    }
                    else if (pi.GetIndexParameters().Length == 0)
                    {
                        ex = Expression.Property(instance, pi);
                    }

                    break;
                case MethodInfo mi:
                    if (IsZeroArityGetterMethod(mi))
                    {
                        ex = mi.IsStatic ? Expression.Call(mi) : Expression.Call(instance, mi);
                    }

                    break;
            }

            return ex;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        private static bool IsZeroArityGetterMethod(MethodInfo mi)
        {
            return mi.GetParameters().Length == 0 && !mi.IsGenericMethod && mi.ReturnType != typeof(void);
        }
    }
}
