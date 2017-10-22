// <copyright file="LiteralTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Stubble.Core.Contexts;
using Stubble.Core.Helpers;
using Stubble.Core.Tokens;

namespace Stubble.Core.Renderers.StringRenderer.TokenRenderers
{
    /// <summary>
    /// A renderer for RawValueTokens
    /// </summary>
    internal class LiteralTokenRenderer : StringObjectRenderer<LiteralToken>
    {
        /// <inheritdoc/>
        protected override void Write(StringRender renderer, LiteralToken obj, Context context)
        {
            for (var i = 0; i < obj.Content.Length; i++)
            {
                var item = obj.Content[i];
                if (obj.Indent > 0 && !item.IsEmptyOrWhitespace())
                {
                    renderer.Write(' ', obj.Indent);
                }

                renderer.Write(ref item);
            }
        }

        /// <inheritdoc/>
        protected override Task WriteAsync(StringRender renderer, LiteralToken obj, Context context)
        {
            Write(renderer, obj, context);
            return TaskHelpers.CompletedTask;
        }
    }
}
