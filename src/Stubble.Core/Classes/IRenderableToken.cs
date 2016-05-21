// <copyright file="IRenderableToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Stubble.Core.Classes
{
    /// <summary>
    /// An interface representing a token which can be rendered
    /// </summary>
    public interface IRenderableToken
    {
        /// <summary>
        /// Returns a tokens rendered reprentation using the given context and partials
        /// </summary>
        /// <param name="writer">The writer to write the token to</param>
        /// <param name="context">The context to discover values from</param>
        /// <param name="partials">The partial templates available to the token</param>
        /// <param name="originalTemplate">The original template</param>
        /// <returns>The rendered result of the token</returns>
        string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate);
    }
}
