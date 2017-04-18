// <copyright file="PartialToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Stubble.Core.Classes.Tokens
{
    /// <summary>
    /// Represents a Partial Token which will load and render a child template
    /// </summary>
    internal class PartialToken : ParserOutput, IRenderableToken
    {
        /// <summary>
        /// Renders a named partial template if one exists with the given context
        /// </summary>
        /// <param name="writer">The writer to write the token to</param>
        /// <param name="context">The context to discover values from</param>
        /// <param name="partials">The partial templates available to the token</param>
        /// <param name="originalTemplate">The original template</param>
        /// <returns>The rendered partial template</returns>
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = partials != null && partials.ContainsKey(Value) ? partials[Value] : null;
            if (value == null && context.RendererSettings.PartialTemplateLoader != null)
            {
                value = context.RendererSettings.PartialTemplateLoader.Load(Value);
            }

            return value != null ? writer.RenderWithOriginalTemplate(value, context, partials, originalTemplate) : null;
        }
    }
}
