// <copyright file="InterpolationToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace Stubble.Core.Classes.Tokens
{
    /// <summary>
    /// An abstract class representing a token which can be interpolated
    /// </summary>
    public abstract class InterpolationToken : ParserOutput
    {
        /// <summary>
        /// Returns the value or if a lambda the result of the lambda (or interpolated value result)
        /// </summary>
        /// <param name="value">the value to be checked and possibly interpolated</param>
        /// <param name="writer">The writer to write the token to</param>
        /// <param name="context">The context to discover values from</param>
        /// <param name="partials">The partial templates available to the token</param>
        /// <returns>The rendered result of the token</returns>
        public object InterpolateLambdaValueIfPossible(object value, Writer writer, Context context, IDictionary<string, string> partials)
        {
            var functionValueDynamic = value as Func<dynamic, object>;
            var functionValue = value as Func<object>;

            if (functionValueDynamic == null && functionValue == null)
            {
                return value;
            }

            object functionResult = functionValueDynamic != null ? functionValueDynamic.Invoke(context.View) : functionValue.Invoke();
            var resultString = functionResult.ToString();
            return resultString.Contains("{{") ? writer.Render(resultString, context, partials) : resultString;
        }
    }
}
