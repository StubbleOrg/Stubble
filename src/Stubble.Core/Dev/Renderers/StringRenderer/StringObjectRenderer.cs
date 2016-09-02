// <copyright file="StringObjectRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;

namespace Stubble.Core.Dev.Renderers
{
    /// <summary>
    /// A renderer which takes a tag and turns it into a string
    /// </summary>
    /// <typeparam name="TToken">The tag type</typeparam>
    public abstract class StringObjectRenderer<TToken> : MustacheTokenRenderer<StringRender, TToken>
        where TToken : ParserOutput
    {
    }
}
