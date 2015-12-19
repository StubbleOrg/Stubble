// <copyright file="EscapedValueToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Net;
using Stubble.Core.Classes.Tokens.Interface;

namespace Stubble.Core.Classes.Tokens
{
    internal class EscapedValueToken : InterpolationToken, IRenderableToken, INonSpace
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);
            value = InterpolateLambdaValueIfPossible(value, writer, context, partials);

            return value != null ? WebUtility.HtmlEncode(value.ToString()) : null;
        }
    }
}
