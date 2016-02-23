// <copyright file="RawValueToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Text;

namespace Stubble.Core.Classes.Tokens
{
    /// <summary>
    /// Represents a raw value which is just rendered
    /// </summary>
    internal class RawValueToken : ParserOutput, IRenderableToken
    {
        /// <summary>
        /// Gets a string builder which is used to build up a raw value
        /// </summary>
        public StringBuilder ValueBuilder { get; } = new StringBuilder();

        /// <summary>
        /// Gets or sets a raw tokens value. On setting it replaces it's value entirely
        /// </summary>
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

        /// <summary>
        /// Returns the raw value of the token
        /// </summary>
        /// <param name="writer">The writer to write the token to</param>
        /// <param name="context">The context to discover values from</param>
        /// <param name="partials">The partial templates available to the token</param>
        /// <param name="originalTemplate">The original template</param>
        /// <returns>The raw tokens value</returns>
        public string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate)
        {
            return Value;
        }
    }
}
