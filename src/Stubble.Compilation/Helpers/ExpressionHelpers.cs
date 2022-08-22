// <copyright file="ExpressionHelpers.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable enable

namespace Stubble.Compilation.Helpers
{
    /// <summary>
    /// A static class containing expression helpers.
    /// </summary>
    internal static class ExpressionHelpers
    {
        /// <summary>
        /// Wraps a nested member expression null checking at every step.
        /// </summary>
        /// <param name="expression">The member expression to check.</param>
        /// <returns>The original expression null checked.</returns>
        public static Expression EnsureSafeAccess(Expression expression)
        {
            var expressions = new Queue<Expression>();
            var item = expression;
            while (item?.NodeType is ExpressionType.MemberAccess or ExpressionType.Call)
            {
                expressions.Enqueue(item);
                item = item switch
                {
                    MethodCallExpression mce => mce.Object,
                    MemberExpression me => me.Expression,
                    _ => throw new InvalidOperationException("Invalid type found"),
                };
            }

            if (expressions.Count is 0)
            {
                return expression;
            }

            var first = expressions.Dequeue();
            var result = WrapWithNullCheckIfRequired(first, first);

            while (expressions.Count > 0)
            {
                var memberExpression = expressions.Dequeue();
                result = WrapWithNullCheckIfRequired(memberExpression, result);
            }

            return result!;

            static Expression WrapWithNullCheckIfRequired(Expression item, Expression @base)
            {
                var parent = item switch
                {
                    MethodCallExpression mce => mce.Object,
                    MemberExpression me => me.Expression,
                    _ => throw new InvalidOperationException("Invalid type found"),
                };

                // Parent is null if it's static and if it's a value type it can't be null
                if (parent is null || parent.Type.IsValueType)
                {
                    return @base;
                }

                return Expression.Condition(
                    Expression.NotEqual(parent, Expression.Default(parent.Type)),
                    @base,
                    Expression.Default(@base.Type));
            }
        }
    }
}
