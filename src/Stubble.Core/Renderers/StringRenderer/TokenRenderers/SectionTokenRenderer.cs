// <copyright file="SectionTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private static readonly List<Type> EnumerableBlacklist = new List<Type>
        {
            typeof(IDictionary),
            typeof(string)
        };

        /// <inheritdoc/>
        protected override void Write(StringRender renderer, SectionToken obj, Context context)
        {
            var value = context.Lookup(obj.SectionName);

            if (!context.IsTruthyValue(value))
            {
                return;
            }

            if (value is IEnumerable && !EnumerableBlacklist.Any(x => x.IsInstanceOfType(value)))
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
            else if (value is Func<dynamic, string, object> || value is Func<string, object>)
            {
                var functionDynamicValue = value as Func<dynamic, string, object>;
                var functionStringValue = value as Func<string, object>;
                var sectionContent = obj.SectionContent;

                value = functionDynamicValue != null
                    ? functionDynamicValue.Invoke(context.View, sectionContent.ToString())
                    : functionStringValue.Invoke(sectionContent.ToString());

                renderer.Render(context.RendererSettings.Parser.Parse(value.ToString(), obj.Tags), context);
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

            if (value is IEnumerable && !EnumerableBlacklist.Any(x => x.IsInstanceOfType(value)))
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
            else if (value is Func<dynamic, string, object> || value is Func<string, object>)
            {
                var functionDynamicValue = value as Func<dynamic, string, object>;
                var functionStringValue = value as Func<string, object>;
                var sectionContent = obj.SectionContent;

                value = functionDynamicValue != null
                    ? functionDynamicValue.Invoke(context.View, sectionContent.ToString())
                    : functionStringValue.Invoke(sectionContent.ToString());

                await renderer.RenderAsync(context.RendererSettings.Parser.Parse(value.ToString(), obj.Tags), context);
            }
            else if (value is IDictionary || value != null)
            {
                await renderer.RenderAsync(obj, context.Push(value));
            }
        }
    }
}
