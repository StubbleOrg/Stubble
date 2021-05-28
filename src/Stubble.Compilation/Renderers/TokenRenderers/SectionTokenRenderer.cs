// <copyright file="SectionTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Stubble.Compilation.Contexts;
using Stubble.Compilation.Helpers;
using Stubble.Compilation.Import;
using Stubble.Core.Tokens;

namespace Stubble.Compilation.Renderers.TokenRenderers
{
    /// <summary>
    /// A <see cref="ExpressionObjectRenderer{SectionToken}"/> for rendering <see cref="SectionToken"/>s
    /// </summary>
    public class SectionTokenRenderer : ExpressionObjectRenderer<SectionToken>
    {
        /// <inheritdoc/>
        protected override void Write(CompilationRenderer renderer, SectionToken obj, CompilerContext context)
        {
            var value = context.Lookup(obj.SectionName);

            if (value == null)
            {
                return;
            }

            Expression expression = null;

            if (typeof(IEnumerable).IsAssignableFrom(value.Type) && !context.CompilerSettings.SectionBlacklistTypes.Any(x => x.IsAssignableFrom(value.Type)))
            {
                var innerType = value.Type.GetElementTypeOfIEnumerable();
                var param = Expression.Parameter(innerType);
                expression = WriteIEnumerable(value, param, renderer.Render(obj, context.Push(innerType, param)) as List<Expression>);
            }
            else if (typeof(IEnumerator).IsAssignableFrom(value.Type))
            {
                var innerType = value.Type.GetElementTypeOfIEnumerable() ?? typeof(object);
                var param = Expression.Parameter(innerType);
                expression = WriteIEnumerator(value, param, innerType, renderer.Render(obj, context.Push(innerType, param)) as List<Expression>);
            }
            else if (typeof(IDictionary).IsAssignableFrom(value.Type) || value != null)
            {
                var param = Expression.Parameter(value.Type);
                var assignment = Expression.Assign(param, value);

                var sectionContent = renderer.Render(obj, context.Push(value.Type, param)) as List<Expression>;
                if (sectionContent.Count > 0)
                {
                    expression = Expression.Block(new[] { param }, new[] { assignment }.Concat(sectionContent));
                }
            }

            if (expression != null)
            {
                var truthy = context.GetTruthyExpression(value);

                var ex = truthy != null
                    ? Expression.IfThen(truthy, expression)
                    : expression;

                renderer.AddExpressionToScope(ex);
            }
        }

        /// <inheritdoc/>
        protected override async Task WriteAsync(CompilationRenderer renderer, SectionToken obj, CompilerContext context)
        {
            var value = context.Lookup(obj.SectionName);

            if (value == null)
            {
                return;
            }

            Expression expression = null;

            if (typeof(IEnumerable).IsAssignableFrom(value.Type) && !context.CompilerSettings.SectionBlacklistTypes.Any(x => x.IsAssignableFrom(value.Type)))
            {
                var innerType = value.Type.GetElementTypeOfIEnumerable();
                var param = Expression.Parameter(innerType);
                var sectionContent = (await renderer.RenderAsync(obj, context.Push(innerType, param))) as List<Expression>;

                if (sectionContent.Count > 0)
                {
                    expression = WriteIEnumerable(value, param, sectionContent);
                }
            }
            else if (typeof(IEnumerator).IsAssignableFrom(value.Type))
            {
                var innerType = value.Type.GetElementTypeOfIEnumerable() ?? typeof(object);
                var param = Expression.Parameter(innerType);
                var sectionContent = (await renderer.RenderAsync(obj, context.Push(innerType, param))) as List<Expression>;

                if (sectionContent.Count > 0)
                {
                    expression = WriteIEnumerator(value, param, innerType, sectionContent);
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(value.Type) || value != null)
            {
                var param = Expression.Parameter(value.Type);
                var assignment = Expression.Assign(param, value);

                var sectionContent = await renderer.RenderAsync(obj, context.Push(value.Type, param)) as List<Expression>;
                if (sectionContent.Count > 0)
                {
                    expression = Expression.Block(new[] { param }, new[] { assignment }.Concat(sectionContent));
                }
            }

            if (expression != null)
            {
                var truthy = context.GetTruthyExpression(value);

                var ex = truthy != null
                    ? Expression.IfThen(truthy, expression)
                    : expression;

                renderer.AddExpressionToScope(ex);
            }
        }

        private static Expression WriteIEnumerable(Expression value, ParameterExpression param, List<Expression> blockContent)
        {
            if (blockContent.Count > 0)
            {
                var blockExpressions = new List<Expression>();
                var continueLabelTarget = Expression.Label("continue");
                var breakLabelTarget = Expression.Label("break");
                if (!param.Type.GetIsValueType())
                {
                    blockExpressions.Add(Expression.IfThen(Expression.Equal(param, Expression.Constant(null)), Expression.Goto(continueLabelTarget)));
                }

                blockExpressions.AddRange(blockContent);

                var block = Expression.Block(blockExpressions);
                return CustomExpression.ForEach(param, value, block, breakLabelTarget, continueLabelTarget);
            }

            return null;
        }

        private static Expression WriteIEnumerator(Expression value, ParameterExpression param, Type type, List<Expression> blockContent)
        {
            if (blockContent.Count > 0)
            {
                var getMethod = Expression.Call(value, MethodInfos.Instance.EnumeratorGetCurrent);

                var whileBlock = Expression.Block(new[] { param }, new[]
                {
                    Expression.Assign(param, type == typeof(object) ? Expression.Convert(getMethod, typeof(object)) : (Expression)getMethod)
                }.Concat(blockContent));

                var block = new Expression[]
                {
                    CustomExpression.While(Expression.Call(value, MethodInfos.Instance.MoveNext), whileBlock),
                    Expression.Call(value, MethodInfos.Instance.EnumeratorReset)
                };

                return Expression.Block(block);
            }

            return null;
        }
    }
}
