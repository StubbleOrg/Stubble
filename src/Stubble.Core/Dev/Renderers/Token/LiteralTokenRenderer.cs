// <copyright file="LiteralTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Dev.Renderers;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Renderers.Token
{
    /// <summary>
    /// A renderer for RawValueTokens
    /// </summary>
    internal class LiteralTokenRenderer : StringObjectRenderer<LiteralTag>
    {
        /// <inheritdoc/>
        protected override void Write(StringRender renderer, LiteralTag obj, Context context)
        {
            var indentStr = new string(' ', obj.Indent);

            foreach (var item in obj.Content)
            {
                if (obj.Indent > 0 && !item.IsEmptyOrWhitespace())
                {
                    renderer.Write(indentStr);
                }

                renderer.Write(item.ToString());
            }
        }
    }
}
