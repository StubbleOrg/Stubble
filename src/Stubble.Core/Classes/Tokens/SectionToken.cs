// <copyright file="SectionToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
#if NETSTANDARD1_3
using System.Reflection;
#endif
using System.Linq;
using System.Text;
using Stubble.Core.Classes.Tokens.Interface;
using Stubble.Core.Helpers;

namespace Stubble.Core.Classes.Tokens
{
    /// <summary>
    /// Represents a block section in a template
    /// </summary>
    internal class SectionToken : ParserOutput, IRenderableToken, ISection
    {
        /// <summary>
        /// Lists the types that should be excluded from being treated like Enumerables
        /// </summary>
        public static readonly List<Type> EnumerableBlacklist = new List<Type>
        {
            typeof(IDictionary),
            typeof(string)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionToken"/> class
        /// with the given <see cref="Tags"/>
        /// </summary>
        /// <param name="tags">The tags to use for block lambda interpolation</param>
        public SectionToken(Tags tags)
        {
            Tags = tags;
        }

        /// <summary>
        /// Gets the tags to use for in rendering block lambdas
        /// </summary>
        public Tags Tags { get; }

        /// <summary>
        /// Returns the rendered result of all the child tokens of the section
        /// if the token value is truthy
        /// </summary>
        /// <param name="writer">The writer to write the token to</param>
        /// <param name="context">The context to discover values from</param>
        /// <param name="partials">The partial templates available to the token</param>
        /// <param name="originalTemplate">The original template</param>
        /// <returns>The rendered result of all the sections children</returns>
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var buffer = new StringBuilder();
            var value = context.Lookup(Value);

            if (!context.IsTruthyValue(value))
            {
                return null;
            }

            if (value is IEnumerable && !EnumerableBlacklist.Any(x => x.IsInstanceOfType(value)))
            {
                var arrayValue = value as IEnumerable;

                foreach (var v in arrayValue)
                {
                    buffer.Append(writer.RenderTokens(ChildTokens, context.Push(v), partials, originalTemplate));
                }
            }
            else if (value is IEnumerator)
            {
                var enumeratorValue = value as IEnumerator;
                while (enumeratorValue.MoveNext())
                {
                    buffer.Append(writer.RenderTokens(ChildTokens, context.Push(enumeratorValue.Current), partials, originalTemplate));
                }
            }
            else if (value is Func<dynamic, string, object> || value is Func<string, object>)
            {
                var functionDynamicValue = value as Func<dynamic, string, object>;
                var functionStringValue = value as Func<string, object>;
                var sectionContent = originalTemplate.Slice(End, ParentSectionEnd);
                value = functionDynamicValue != null ? functionDynamicValue.Invoke(context.View, sectionContent) : functionStringValue.Invoke(sectionContent);
                value = writer.Render(value.ToString(), context, partials, Tags);

                if (value != null)
                {
                    buffer.Append(value);
                }
            }
            else if (value is IDictionary || value != null)
            {
                buffer.Append(writer.RenderTokens(ChildTokens, context.Push(value), partials, originalTemplate));
            }

            return buffer.ToString();
        }
    }
}
