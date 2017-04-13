// <copyright file="PartialTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Dev.Parser;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Renderers.Token
{
    /// <summary>
    /// A renderer for <see cref="PartialTag"/>
    /// </summary>
    internal class PartialTokenRenderer : StringObjectRenderer<PartialTag>
    {
        /// <inheritdoc/>
        protected override void Write(StringRender renderer, PartialTag obj, Context context)
        {
            var partialName = obj.Content;
            string template = null;
            if (context.PartialLoader != null)
            {
                template = context.PartialLoader.Load(partialName.ToString());
            }

            if (template != null)
            {
                renderer.Render(MustacheParser.Parse(template, lineIndent: obj.LineIndent), context);
            }
        }
    }
}
