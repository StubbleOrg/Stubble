﻿// <copyright file="PartialTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Stubble.Compilation.Contexts;
using Stubble.Core.Exceptions;
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
                var sourceDatas = context.GetNestedSourceData().ToArray();
                var key = PartialLambdaExpressionDefinition.GetKey(template, context.SourceData.Type);

                // Recursive calls use the existing variable to call the partial lambda.
                if (renderer.PartialExpressionCache.TryGetValue(key, out var partialVariable))
                {
                    renderer.AddExpressionToScope(Expression.Invoke(partialVariable.Variable, context.SourceData));
                    return;
                }

                var sourceData = sourceDatas.Select(s => s.Type).ToArray();

                if (sourceData.Length > 16)
                {
                    throw new StubbleException("Cannot call a partial with more than 16 parameters.\nThis is likely due to a large amount of section scopes");
                }

                var actionType = Expression.GetActionType(sourceData);

                var definition = new PartialLambdaExpressionDefinition
                {
                    Variable = Expression.Parameter(actionType)
                };

                renderer.PartialExpressionCache.Add(key, definition);

                var partialContent = renderer.Render(
                    context.CompilerSettings.Parser.Parse(
                        template,
                        lineIndent: obj.LineIndent,
                        pipeline: context.CompilerSettings.ParserPipeline),
                    context) as List<Expression>;

                renderer.AddExpressionToScope(AddLambdaDefinition(definition, partialContent, actionType, sourceDatas));
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
                var sourceDatas = context.GetNestedSourceData().ToArray();
                var key = PartialLambdaExpressionDefinition.GetKey(template, context.SourceData.Type);

                // Recursive calls use the existing variable to call the partial lambda.
                if (renderer.PartialExpressionCache.TryGetValue(key, out var partialVariable))
                {
                    renderer.AddExpressionToScope(Expression.Invoke(partialVariable.Variable, context.SourceData));
                    return;
                }

                var sourceData = sourceDatas.Select(s => s.Type).ToArray();

                if (sourceData.Length > 16)
                {
                    throw new StubbleException("Cannot call a partial with more than 16 parameters.\nThis is likely due to a large amount of section scopes");
                }

                var actionType = Expression.GetActionType(sourceData);

                var definition = new PartialLambdaExpressionDefinition
                {
                    Variable = Expression.Parameter(actionType)
                };

                renderer.PartialExpressionCache.Add(key, definition);

                var partialContent = await renderer.RenderAsync(
                    context.CompilerSettings.Parser.Parse(
                        template,
                        lineIndent: obj.LineIndent,
                        pipeline: context.CompilerSettings.ParserPipeline),
                    context) as List<Expression>;

                renderer.AddExpressionToScope(AddLambdaDefinition(definition, partialContent, actionType, sourceDatas));
            }
        }

        private static Expression AddLambdaDefinition(PartialLambdaExpressionDefinition definition, List<Expression> partialContent, Type actionType, Expression[] @params)
        {
            var convertedParams = @params.Select(param =>
            {
                return param is ParameterExpression
                    ? (ParameterExpression)param
                    : Expression.Parameter(param.Type);
            });

            var block = partialContent.Count > 0
                    ? Expression.Block(partialContent)
                    : EmptyPartial;

            definition.Partial = Expression.Lambda(actionType, block, convertedParams);

            return Expression.Invoke(definition.Variable, convertedParams);
        }
    }
}
