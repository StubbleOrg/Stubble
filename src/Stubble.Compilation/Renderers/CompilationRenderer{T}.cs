// <copyright file="CompilationRenderer{T}.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Stubble.Compilation.Contexts;
using Stubble.Compilation.Helpers;
using Stubble.Core.Renderers;
using Stubble.Core.Tokens;

namespace Stubble.Compilation.Renderers
{
    /// <summary>
    /// A renderer for taking types and templates and turning them into functions
    /// </summary>
    /// <typeparam name="T">The type of data to return</typeparam>
    public class CompilationRenderer<T> : CompilationRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationRenderer{T}"/> class.
        /// </summary>
        /// <param name="rendererPipeline">The pipeline for the renderer</param>
        /// <param name="maxDepth">The max depth of recursion the renderer can go</param>
        public CompilationRenderer(TokenRendererPipeline<CompilerContext> rendererPipeline, uint maxDepth)
            : base(rendererPipeline, maxDepth)
        {
        }

        /// <summary>
        /// Compiles to documen with the given compilation conext
        /// </summary>
        /// <param name="document">The document to render</param>
        /// <param name="compilationContext">The context to render with</param>
        /// <returns>A function representing the document</returns>
        public Func<T, string> Compile(MustacheTemplate document, CompilerContext compilationContext)
        {
            var blockContext = (List<Expression>)Render(document, compilationContext);

            return BuildLambda(compilationContext, blockContext);
        }

        /// <summary>
        /// Compiles to documen with the given compilation conext
        /// </summary>
        /// <param name="document">The document to render</param>
        /// <param name="compilationContext">The context to render with</param>
        /// <returns>A function representing the document</returns>
        public async ValueTask<Func<T, string>> CompileAsync(MustacheTemplate document, CompilerContext compilationContext)
        {
            var blockContext = (List<Expression>)(await RenderAsync(document, compilationContext));

            return BuildLambda(compilationContext, blockContext);
        }

        private Func<T, string> BuildLambda(CompilerContext compilationContext, List<Expression> blockContext)
        {
            var @params = new List<ParameterExpression>(1 + PartialExpressionCache.Count)
            {
                Builder
            };

            foreach (var partial in PartialExpressionCache)
            {
                Expressions.Add(Expression.Assign(partial.Value.Variable, partial.Value.Partial));
                @params.Add(partial.Value.Variable);
            }

            if (blockContext.Count > 0)
            {
                Expressions.Add(Expression.Block(blockContext));
            }

            Expressions.Add(Expression.Call(Builder, MethodInfos.Instance.StringBuilderToString));

            var param = (ParameterExpression)compilationContext.SourceData;

            var lambda = Expression.Lambda<Func<T, string>>(Expression.Block(@params, Expressions), param);

            return lambda.Compile();
        }
    }
}
