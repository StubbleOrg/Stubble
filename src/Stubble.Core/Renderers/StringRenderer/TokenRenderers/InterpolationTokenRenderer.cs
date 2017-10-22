// <copyright file="InterpolationTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Threading.Tasks;
using Stubble.Core.Contexts;
using Stubble.Core.Tokens;

namespace Stubble.Core.Renderers.StringRenderer.TokenRenderers
{
    /// <summary>
    /// A <see cref="StringObjectRenderer{InterpolationTag}"/> for rendering <see cref="InterpolationToken"/>s
    /// </summary>
    public class InterpolationTokenRenderer : StringObjectRenderer<InterpolationToken>
    {
        /// <inheritdoc/>
        protected override void Write(StringRender renderer, InterpolationToken obj, Context context)
        {
            var value = context.Lookup(obj.Content.ToString());

            var functionValueDynamic = value as Func<dynamic, object>;
            var functionValue = value as Func<object>;

            if (functionValueDynamic != null || functionValue != null)
            {
                object functionResult = functionValueDynamic != null ? functionValueDynamic.Invoke(context.View) : functionValue.Invoke();
                var resultString = functionResult.ToString();
                if (resultString.Contains("{{"))
                {
                    renderer.Render(context.RendererSettings.Parser.Parse(resultString), context);
                    return;
                }

                value = resultString;
            }

            if (obj.EscapeResult && value != null)
            {
                value = WebUtility.HtmlEncode(value.ToString());
            }

            if (obj.Indent > 0)
            {
                renderer.Write(' ', obj.Indent);
            }

            renderer.Write(value?.ToString());
        }

        /// <inheritdoc/>
        protected override async Task WriteAsync(StringRender renderer, InterpolationToken obj, Context context)
        {
            var value = context.Lookup(obj.Content.ToString());

            var functionValueDynamic = value as Func<dynamic, object>;
            var functionValue = value as Func<object>;

            if (functionValueDynamic != null || functionValue != null)
            {
                object functionResult = functionValueDynamic != null ? functionValueDynamic.Invoke(context.View) : functionValue.Invoke();
                var resultString = functionResult.ToString();
                if (resultString.Contains("{{"))
                {
                    await renderer.RenderAsync(context.RendererSettings.Parser.Parse(resultString), context);
                    return;
                }

                value = resultString;
            }

            if (obj.EscapeResult && value != null)
            {
                value = WebUtility.HtmlEncode(value.ToString());
            }

            if (obj.Indent > 0)
            {
                renderer.Write(' ', obj.Indent);
            }

            renderer.Write(value?.ToString());
        }
    }
}
