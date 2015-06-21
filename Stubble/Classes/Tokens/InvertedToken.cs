﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Helpers;

namespace Stubble.Core.Classes.Tokens
{
    public class InvertedToken : InterpolationToken, IRenderableToken
    {
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            var value = context.Lookup(Value);
            value = InterpolateLambdaValueIfPossible(value, writer, context, partials);

            return !ValueHelpers.IsTruthy(value) ? writer.RenderTokens(ChildTokens, context, partials, originalTemplate) : null;
        }
    }
}