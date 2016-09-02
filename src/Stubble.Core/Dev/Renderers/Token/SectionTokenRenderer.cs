// <copyright file="SectionTokenRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using Stubble.Core.Classes.Tokens;

namespace Stubble.Core.Dev.Renderers.Token
{
    /// <summary>
    /// A <see cref="StringObjectRenderer{SectionToken}"/> for rendering section tokens
    /// </summary>
    internal class SectionTokenRenderer : StringObjectRenderer<SectionToken>
    {
        private static readonly List<Type> EnumerableBlacklist = new List<Type>
        {
            typeof(IDictionary),
            typeof(string)
        };

        /// <inheritdoc/>
        protected override void Write(StringRender renderer, SectionToken obj)
        {
            return;
        }
    }
}
