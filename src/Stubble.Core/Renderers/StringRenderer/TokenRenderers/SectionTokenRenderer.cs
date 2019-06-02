// <copyright file="SectionTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Stubble.Core.Contexts;
using Stubble.Core.Tokens;

namespace Stubble.Core.Renderers.StringRenderer.TokenRenderers
{
    /// <summary>
    /// A <see cref="StringObjectRenderer{SectionToken}"/> for rendering section tokens
    /// </summary>
    internal class SectionTokenRenderer : StringObjectRenderer<SectionToken>
    {
        private static readonly HashSet<Type> LambdaTypes = new HashSet<Type>
        {
            typeof(Func<dynamic, string, object>),
            typeof(Func<string, object>),
            typeof(Func<dynamic, string, Func<string, string>, object>),
            typeof(Func<string, Func<string, string>, object>)
        };

        /// <inheritdoc/>
        protected override void Write(StringRender renderer, SectionToken obj, Context context)
        {
            var value = context.Lookup(obj.SectionName);

            if (!context.IsTruthyValue(value))
            {
                return;
            }

            if (value is IEnumerable && !context.RendererSettings.SectionBlacklistTypes.Any(x => x.IsInstanceOfType(value)))
            {
                var arrayValue = value as IEnumerable;

                foreach (var v in arrayValue)
                {
                    renderer.Render(obj, context.Push(v));
                }
            }
            else if (value is IEnumerator)
            {
                var enumeratorValue = value as IEnumerator;
                while (enumeratorValue.MoveNext())
                {
                    renderer.Render(obj, context.Push(enumeratorValue.Current));
                }

                enumeratorValue.Reset();
            }
            else if (LambdaTypes.Contains(value.GetType()))
            {
                var sectionContent = obj.SectionContent.ToString();

                switch (value)
                {
                    case Func<dynamic, string, object> func:
                        value = func(context.View, sectionContent);
                        break;
                    case Func<string, object> func:
                        value = func(sectionContent);
                        break;
                    case Func<dynamic, string, Func<string, string>, object> func:
                        value = func(context.View, sectionContent, RenderInContext(context, obj.Tags));
                        break;
                    case Func<string, Func<string, string>, object> func:
                        value = func(sectionContent, RenderInContext(context, obj.Tags));
                        break;
                }

                var valueString = value?.ToString() ?? string.Empty;
                renderer.Render(context.RendererSettings.Parser.Parse(valueString, obj.Tags), context);
            }
            else if (value is IDictionary || value != null)
            {
                renderer.Render(obj, context.Push(value));
            }
        }

        /// <inheritdoc/>
        protected override async Task WriteAsync(StringRender renderer, SectionToken obj, Context context)
        {
            var value = context.Lookup(obj.SectionName);

            if (!context.IsTruthyValue(value))
            {
                return;
            }

            if (value is IEnumerable && !context.RendererSettings.SectionBlacklistTypes.Any(x => x.IsInstanceOfType(value)))
            {
                var arrayValue = value as IEnumerable;

                foreach (var v in arrayValue)
                {
                    await renderer.RenderAsync(obj, context.Push(v));
                }
            }
            else if (value is IEnumerator)
            {
                var enumeratorValue = value as IEnumerator;
                while (enumeratorValue.MoveNext())
                {
                    await renderer.RenderAsync(obj, context.Push(enumeratorValue.Current));
                }

                enumeratorValue.Reset();
            }
            else if (LambdaTypes.Contains(value.GetType()))
            {
                var sectionContent = obj.SectionContent.ToString();

                switch (value)
                {
                    case Func<dynamic, string, object> func:
                        value = func(context.View, sectionContent);
                        break;
                    case Func<string, object> func:
                        value = func(sectionContent);
                        break;
                    case Func<dynamic, string, Func<string, string>, object> func:
                        value = func(context.View, sectionContent, RenderInContext(context, obj.Tags));
                        break;
                    case Func<string, Func<string, string>, object> func:
                        value = func(sectionContent, RenderInContext(context, obj.Tags));
                        break;
                }

                var valueString = value?.ToString() ?? string.Empty;
                renderer.Render(context.RendererSettings.Parser.Parse(value.ToString(), obj.Tags), context);
            }
            else if (value is IDictionary || value != null)
            {
                await renderer.RenderAsync(obj, context.Push(value));
            }
        }

        private Func<string, string> RenderInContext(Context context, Classes.Tags tags)
        {
            return (str) =>
            {
                if (!str.Contains(tags.StartTag))
                {
                    return str;
                }

                using (var writer = new StringWriter())
                {
                    var blockRenderer = new StringRender(writer, context.RendererSettings.RendererPipeline);
                    var parsed = context.RendererSettings.Parser.Parse(str, tags);

                    blockRenderer.Render(parsed, context);

                    return writer.ToString();
                }
            };
        }
    }
}
