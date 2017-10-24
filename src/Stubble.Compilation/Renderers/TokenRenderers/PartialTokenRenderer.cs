// <copyright file="PartialTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Stubble.Compilation.Contexts;
using Stubble.Core.Tokens;

namespace Stubble.Compilation.Renderers.TokenRenderers
{
    /// <summary>
    /// A <see cref="ExpressionObjectRenderer{PartialToken}"/> for rendering <see cref="PartialToken"/>s
    /// </summary>
    public class PartialTokenRenderer : ExpressionObjectRenderer<PartialToken>
    {
        private static readonly Expression EmptyPartial = Expression.Block(Expression.Empty());

        /// <inheritdoc/>
        protected override void Write(CompilationRenderer renderer, PartialToken obj, CompilerContext context)
        {
            var partialName = obj.Content;
            string template = null;
            if (context.PartialLoader != null)
            {
                template = context.PartialLoader.Load(partialName.ToString());
            }

            if (template != null)
            {
                // Recursive calls use the existing variable to call the partial lambda.
                if (renderer.PartialExpressionCache.TryGetValue(template, out PartialLambdaExpressionDefinition var))
                {
                    renderer.AddExpressionToScope(Expression.Invoke(var.Variable, context.SourceData));
                    return;
                }

                var actionType = typeof(Action<>).MakeGenericType(context.SourceData.Type);

                var definition = new PartialLambdaExpressionDefinition
                {
                    Variable = Expression.Parameter(actionType)
                };

                renderer.PartialExpressionCache.Add(template, definition);

                var partialContent = renderer.Render(context.CompilerSettings.Parser.Parse(template, lineIndent: obj.LineIndent), context) as List<Expression>;

                renderer.AddExpressionToScope(AddLambdaDefinition(definition, partialContent, actionType, (ParameterExpression)context.SourceData));
            }
        }

        /// <inheritdoc/>
        protected override async Task WriteAsync(CompilationRenderer renderer, PartialToken obj, CompilerContext context)
        {
            var partialName = obj.Content;
            string template = null;
            if (context.PartialLoader != null)
            {
                template = await context.PartialLoader.LoadAsync(partialName.ToString());
            }

            if (template != null)
            {
                // Recursive calls use the existing variable to call the partial lambda.
                if (renderer.PartialExpressionCache.TryGetValue(template, out PartialLambdaExpressionDefinition var))
                {
                    renderer.AddExpressionToScope(Expression.Invoke(var.Variable, context.SourceData));
                    return;
                }

                var actionType = typeof(Action<>).MakeGenericType(context.SourceData.Type);

                var definition = new PartialLambdaExpressionDefinition
                {
                    Variable = Expression.Parameter(actionType)
                };

                renderer.PartialExpressionCache.Add(template, definition);

                var partialContent = (await renderer.RenderAsync(context.CompilerSettings.Parser.Parse(template, lineIndent: obj.LineIndent), context)) as List<Expression>;

                renderer.AddExpressionToScope(AddLambdaDefinition(definition, partialContent, actionType, (ParameterExpression)context.SourceData));
            }
        }

        private static Expression AddLambdaDefinition(PartialLambdaExpressionDefinition definition, List<Expression> partialContent, Type actionType, ParameterExpression param)
        {
            Expression block = partialContent.Count > 0
                    ? Expression.Block(partialContent)
                    : EmptyPartial;

            definition.Partial = Expression.Lambda(actionType, block, new[] { param });

            return Expression.Invoke(definition.Variable, param);
        }
    }
}
