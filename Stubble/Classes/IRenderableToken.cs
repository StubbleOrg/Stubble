// <copyright file="IRenderableToken.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Stubble.Core.Classes
{
    public interface IRenderableToken
    {
        string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate);
    }
}
