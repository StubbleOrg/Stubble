// <copyright file="MustacheTemplate.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Dev.Tags
{
    /// <summary>
    /// The root of a mustache template
    /// </summary>
    public class MustacheTemplate : BlockTag
    {
        /// <inheritdoc/>
        public override string Identifier => "Root";
    }
}
