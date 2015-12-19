// <copyright file="InvertedToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Classes.Tokens.Interface;
using Stubble.Core.Helpers;

namespace Stubble.Core.Classes.Tokens
{
    internal class InvertedToken : InterpolationToken, IRenderableToken, ISection
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);
            value = InterpolateLambdaValueIfPossible(value, writer, context, partials);

            return !context.IsTruthyValue(value) ? writer.RenderTokens(ChildTokens, context, partials, originalTemplate) : null;
        }
    }
}
