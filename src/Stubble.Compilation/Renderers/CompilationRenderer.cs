// <copyright file="CompilationRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Stubble.Compilation.Contexts;
using Stubble.Core.Exceptions;
using Stubble.Core.Renderers;
using Stubble.Core.Tokens;

namespace Stubble.Compilation.Renderers
{
    /// <summary>
    /// A non typed compilation renderer
    /// </summary>
    public class CompilationRenderer : RendererBase<CompilerContext>
    {
        private uint currentDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationRenderer"/> class.
        /// </summary>
        /// <param name="rendererPipeline">The pipeline for the renderer</param>
        /// <param name="maxDepth">The max depth of recursion the renderer can go</param>
        protected CompilationRenderer(TokenRendererPipeline<CompilerContext> rendererPipeline, uint maxDepth)
            : base(rendererPipeline)
        {
            MaxDepth = maxDepth;
            Builder = Expression.Variable(typeof(StringBuilder), "sb");
            Expressions = new List<Expression>()
            {
                Expression.Assign(Builder, Expression.New(typeof(StringBuilder)))
            };
        }

        /// <summary>
        /// Gets a parameter representing the StringBuilder for the renderer
        /// </summary>
        public ParameterExpression Builder { get; }

        /// <summary>
        /// Gets a temporary cache containing partial expressions and references to their declaration
        /// </summary>
        public Dictionary<string, PartialLambdaExpressionDefinition> PartialExpressionCache { get; }
            = new Dictionary<string, PartialLambdaExpressionDefinition>();

        /// <summary>
        /// Gets the MaxDepth for the renderer
        /// </summary>
        protected uint MaxDepth { get; }

        /// <summary>
        /// Gets the list of expressions
        /// </summary>
        protected List<Expression> Expressions { get; }

        /// <summary>
        /// Gets a stack containing the different render scopes
        /// </summary>
        protected Stack<List<Expression>> Scopes { get; } = new Stack<List<Expression>>();

        /// <summary>
        /// Renders the block with the given context
        /// </summary>
        /// <param name="block">The block to render</param>
        /// <param name="context">The context to write the block with</param>
        /// <returns>A list of tokens in the scope</returns>
        public override object Render(BlockToken block, CompilerContext context)
        {
            Scopes.Push(new List<Expression>());
            foreach (var tag in block.Children)
            {
                currentDepth++;
                if (currentDepth >= MaxDepth)
                {
                    throw new StubbleException(
                        $"You have reached the maximum recursion limit of {MaxDepth}.");
                }

                Write(tag, context);
                currentDepth--;
            }

            var scope = Scopes.Pop();

            return scope;
        }

        /// <summary>
        /// Renders the block with the given context
        /// </summary>
        /// <param name="block">The block to render</param>
        /// <param name="context">The context to write the block with</param>
        /// <returns>A list of tokens in the scope</returns>
        public override async ValueTask<object> RenderAsync(BlockToken block, CompilerContext context)
        {
            Scopes.Push(new List<Expression>());
            foreach (var tag in block.Children)
            {
                currentDepth++;
                if (currentDepth >= MaxDepth)
                {
                    throw new StubbleException(
                        $"You have reached the maximum recursion limit of {MaxDepth}.");
                }

                await WriteAsync(tag, context);
                currentDepth--;
            }

            var scope = Scopes.Pop();

            return scope;
        }

        /// <summary>
        /// Adds the expression to a scope if active or the global expressions list if not
        /// </summary>
        /// <param name="expression">The expression to add</param>
        public void AddExpressionToScope(Expression expression)
        {
            if (Scopes.Count > 0)
            {
                Scopes.Peek().Add(expression);
                return;
            }

            Expressions.Add(expression);
        }
    }
}
