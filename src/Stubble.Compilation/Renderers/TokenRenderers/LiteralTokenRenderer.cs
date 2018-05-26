// <copyright file="LiteralTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Stubble.Compilation.Contexts;
using Stubble.Compilation.Helpers;
using Stubble.Core.Helpers;
using Stubble.Core.Tokens;

namespace Stubble.Compilation.Renderers.TokenRenderers
{
    /// <summary>
    /// A <see cref="ExpressionObjectRenderer{LiteralToken}"/> for rendering <see cref="LiteralToken"/>s
    /// </summary>
    public class LiteralTokenRenderer : ExpressionObjectRenderer<LiteralToken>
    {
        /// <inheritdoc/>
        protected override void Write(CompilationRenderer renderer, LiteralToken obj, CompilerContext context)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < obj.Content.Length; i++)
            {
                var item = obj.Content[i];
                if (obj.Indent > 0 && !item.IsEmptyOrWhitespace())
                {
                    builder.Append(' ', obj.Indent);
                }

                builder.Append(item.ToString());
            }

            var append = Expression.Call(renderer.Builder, MethodInfos.Instance.StringBuilderAppendString, Expression.Constant(builder.ToString()));

            renderer.AddExpressionToScope(append);
        }

        /// <inheritdoc/>
        protected override Task WriteAsync(CompilationRenderer renderer, LiteralToken obj, CompilerContext context)
        {
            Write(renderer, obj, context);
            return TaskHelpers.CompletedTask;
        }
    }
}
