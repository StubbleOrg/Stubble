// <copyright file="PartialTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Tokens;

namespace Stubble.Core.Renderers.StringRenderer.TokenRenderers
{
    /// <summary>
    /// A renderer for <see cref="PartialToken"/>
    /// </summary>
    internal class PartialTokenRenderer : StringObjectRenderer<PartialToken>
    {
        /// <inheritdoc/>
        protected override void Write(StringRender renderer, PartialToken obj, Context context)
        {
            var partialName = obj.Content;
            string template = null;
            if (context.PartialLoader != null)
            {
                template = context.PartialLoader.Load(partialName.ToString());
            }

            if (template != null)
            {
                renderer.Render(context.RendererSettings.Parser.Parse(template, lineIndent: obj.LineIndent), context);
            }
        }
    }
}
