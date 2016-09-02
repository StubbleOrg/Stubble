// <copyright file="RawValueTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes.Tokens;
using Stubble.Core.Dev.Renderers;

namespace Stubble.Core.Dev.Renderer
{
    /// <summary>
    /// A renderer for RawValueTokens
    /// </summary>
    internal class RawValueTokenRenderer : StringObjectRenderer<RawValueToken>
    {
        /// <inheritdoc/>
        protected override void Write(StringRender renderer, RawValueToken obj)
        {
            renderer.Write(obj.Value);
        }
    }
}
