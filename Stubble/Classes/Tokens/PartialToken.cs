// <copyright file="PartialToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Stubble.Core.Classes.Tokens
{
    internal class PartialToken : ParserOutput, IRenderableToken
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = partials != null && partials.ContainsKey(Value) ? partials[Value] : null;
            if (value == null && context.Registry.PartialTemplateLoader != null)
            {
                value = context.Registry.PartialTemplateLoader.Load(Value);
            }

            return value != null ? writer.RenderTokens(writer.Parse(value), context, partials, value) : null;
        }
    }
}
