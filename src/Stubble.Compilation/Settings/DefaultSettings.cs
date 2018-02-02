// <copyright file="DefaultSettings.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Stubble.Compilation.Contexts;
using Stubble.Compilation.Renderers.TokenRenderers;
using Stubble.Core.Helpers;
using Stubble.Core.Renderers.Interfaces;

namespace Stubble.Compilation.Settings
{
    /// <summary>
    /// Contains defaults for the <see cref="CompilerSettings"/>
    /// </summary>
    internal static class DefaultSettings
    {
        /// <summary>
        /// Returns the default value getters
        /// </summary>
        /// <param name="ignoreCase">If case should be ignored on lookup</param>
        /// <returns>The default value getters</returns>
        public static Dictionary<Type, Func<Type, Expression, string, Expression>> DefaultValueGetters(
            bool ignoreCase)
        {
            return new Dictionary<Type, Func<Type, Expression, string, Expression>>()
            {
                {
                    typeof(IList),
                    (type, instance, key) =>
                    {
                        if (int.TryParse(key, out int intVal))
                        {
                            var index = Expression.Constant(intVal);

                            return Expression.Condition(
                                Expression.LessThan(index, Expression.Property(instance, typeof(ICollection), "Count")),
                                Expression.MakeIndex(instance, typeof(IList).GetProperty("Item"), new[] { index }),
                                Expression.Constant(null));
                        }

                        return null;
                    }
                },
                {
                    typeof(IDictionary<string, object>),
                    (type, instance, key) =>
                    {
                        var outVar = Expression.Variable(typeof(object));

                        return Expression.Block(new Expression[]
                        {
                            outVar,
                            Expression.Condition(
                                Expression.Call(instance, type.GetMethod("TryGetValue"), new Expression[] { Expression.Constant(key), outVar }),
                                outVar,
                                Expression.Constant(null))
                        });
                    }
                },
                {
                    typeof(IDictionary),
                    (type, instance, key) =>
                    {
                        return Expression.MakeIndex(instance, typeof(IDictionary).GetProperty("Item"), new[] { Expression.Constant(key) });
                    }
                },
                {
                    typeof(object), (type, instance, key) =>
                    {
                        var member = type.GetMember(
                            key,
                            BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);

                        if (member.Length > 0)
                        {
                            var info = member[0];
                            if (!ignoreCase && !info.Name.Equals(key, StringComparison.Ordinal))
                            {
                                return null;
                            }

                            var expression = ReflectionHelper.GetExpressionFromMemberInfo(info, instance);
                            return expression;
                        }

                        return null;
                    }
                }
            };
        }

        /// <summary>
        /// Returns the default token renderers
        /// </summary>
        /// <returns>Default token renderers</returns>
        internal static IEnumerable<ITokenRenderer<CompilerContext>> DefaultTokenRenderers()
        {
            return new List<ITokenRenderer<CompilerContext>>
             {
                new SectionTokenRenderer(),
                new LiteralTokenRenderer(),
                new InterpolationTokenRenderer(),
                new PartialTokenRenderer(),
                new InvertedSectionTokenRenderer(),
             };
        }
    }
}
