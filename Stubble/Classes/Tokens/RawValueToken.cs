// <copyright file="RawValueToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Text;

namespace Stubble.Core.Classes.Tokens
{
    internal class RawValueToken : ParserOutput, IRenderableToken
    {
        public StringBuilder ValueBuilder { get; } = new StringBuilder();

        public override string Value
        {
            get
            {
                return ValueBuilder.ToString();
            }

            set
            {
                ValueBuilder.Clear().Append(value);
            }
        }

        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            return Value;
        }
    }
}
