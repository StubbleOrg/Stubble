// <copyright file="InvertedToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Classes.Tokens.Interface;

namespace Stubble.Core.Classes.Tokens
{
    /// <summary>
    /// Represents an inverted section which is only rendered if the value is not truthy
    /// </summary>
    internal class InvertedToken : InterpolationToken, IRenderableToken, ISection
    {
        /// <summary>
        /// Renders a tokens representation if the value (or interpolated value result)
        /// is truthy
        /// </summary>
        /// <param name="writer">The writer to write the token to</param>
        /// <param name="context">The context to discover values from</param>
        /// <param name="partials">The partial templates available to the token</param>
        /// <param name="originalTemplate">The original template</param>
        /// <returns>The rendered result of the token if the resolved value is not truthy</returns>
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);
            value = InterpolateLambdaValueIfPossible(value, writer, context, partials);

            return !context.IsTruthyValue(value) ? writer.RenderTokens(ChildTokens, context, partials, originalTemplate) : null;
        }
    }
}
