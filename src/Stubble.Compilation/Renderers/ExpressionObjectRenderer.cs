// <copyright file="ExpressionObjectRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Compilation.Contexts;
using Stubble.Core.Renderers;
using Stubble.Core.Tokens;

namespace Stubble.Compilation.Renderers
{
    /// <summary>
    /// A renderer which takes a tag and turns it into a string
    /// </summary>
    /// <typeparam name="TToken">The tag type</typeparam>
    public abstract class ExpressionObjectRenderer<TToken> : MustacheTokenRenderer<CompilationRenderer, TToken, CompilerContext>
        where TToken : MustacheToken
    {
    }
}
