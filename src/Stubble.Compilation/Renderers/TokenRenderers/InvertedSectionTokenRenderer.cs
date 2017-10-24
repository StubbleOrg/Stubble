// <copyright file="InvertedSectionTokenRenderer.cs" company="Stubble Authors">
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
    /// A <see cref="ExpressionObjectRenderer{InvertedSectionToken}"/> for rendering <see cref="InvertedSectionToken"/>s
    /// </summary>
    public class InvertedSectionTokenRenderer : ExpressionObjectRenderer<InvertedSectionToken>
    {
        /// <inheritdoc/>
        protected override void Write(CompilationRenderer renderer, InvertedSectionToken obj, CompilerContext context)
        {
            Expression expression = null;
            var sectionContent = renderer.Render(obj, context) as List<Expression>;

            if (sectionContent != null && sectionContent.Count > 0)
            {
                expression = Expression.Block(sectionContent);
            }

            var value = context.Lookup(obj.SectionName);

            if (value != null && expression != null)
            {
                expression = Expression.IfThen(Expression.Not(context.GetTruthyExpression(value)), expression);
            }

            if (expression != null)
            {
                renderer.AddExpressionToScope(expression);
            }
        }

        /// <inheritdoc/>
        protected override async Task WriteAsync(CompilationRenderer renderer, InvertedSectionToken obj, CompilerContext context)
        {
            Expression expression = null;
            var sectionContent = await renderer.RenderAsync(obj, context) as List<Expression>;

            if (sectionContent != null && sectionContent.Count > 0)
            {
                expression = Expression.Block(sectionContent);
            }

            var value = context.Lookup(obj.SectionName);

            if (value != null && expression != null)
            {
                expression = Expression.IfThen(Expression.Not(context.GetTruthyExpression(value)), expression);
            }

            if (expression != null)
            {
                renderer.AddExpressionToScope(expression);
            }
        }
    }
}
