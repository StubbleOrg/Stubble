// <copyright file="InterpolationTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using Stubble.Core.Dev.Parser;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Renderers.Token
{
    /// <summary>
    /// A <see cref="StringObjectRenderer{InterpolationTag}"/> for rendering <see cref="InterpolationTag"/>s
    /// </summary>
    public class InterpolationTokenRenderer : StringObjectRenderer<InterpolationTag>
    {
        /// <inheritdoc/>
        protected override void Write(StringRender renderer, InterpolationTag obj, Context context)
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
                renderer.Write(new string(' ', obj.Indent));
            }

            renderer.Write(value?.ToString());
        }
    }
}
