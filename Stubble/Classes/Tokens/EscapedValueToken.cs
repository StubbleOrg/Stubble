// <copyright file="EscapedValueToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Net;
using Stubble.Core.Classes.Tokens.Interface;

namespace Stubble.Core.Classes.Tokens
{
    /// <summary>
    /// Represents a Value Token which has been escaped
    /// </summary>
    internal class EscapedValueToken : InterpolationToken, IRenderableToken, INonSpace
    {

        /// <summary>
        /// Renders a tokens representation (or interpolated lambda result) escaping the
        /// result
        /// </summary>
        /// <param name="writer">The writer to write the token to</param>
        /// <param name="context">The context to discover values from</param>
        /// <param name="partials">The partial templates available to the token</param>
        /// <param name="originalTemplate">The original template</param>
        /// <returns>The rendered token result that has been escaped</returns>
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);
            value = InterpolateLambdaValueIfPossible(value, writer, context, partials);

            return value != null ? WebUtility.HtmlEncode(value.ToString()) : null;
        }
    }
}
