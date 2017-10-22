// <copyright file="InvertedSectionTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Stubble.Core.Contexts;
using Stubble.Core.Tokens;

namespace Stubble.Core.Renderers.StringRenderer.TokenRenderers
{
    /// <summary>
    /// A <see cref="StringObjectRenderer{InvertedSectionTag}"/> for rendering section tokens
    /// </summary>
    internal class InvertedSectionTokenRenderer : StringObjectRenderer<InvertedSectionToken>
    {
        /// <inheritdoc/>
        protected override void Write(StringRender renderer, InvertedSectionToken obj, Context context)
        {
            var value = context.Lookup(obj.SectionName);

            if (!context.IsTruthyValue(value))
            {
                renderer.Render(obj, context);
            }
        }

        /// <inheritdoc/>
        protected override async Task WriteAsync(StringRender renderer, InvertedSectionToken obj, Context context)
        {
            var value = context.Lookup(obj.SectionName);

            if (!context.IsTruthyValue(value))
            {
                await renderer.RenderAsync(obj, context);
            }
        }
    }
}
