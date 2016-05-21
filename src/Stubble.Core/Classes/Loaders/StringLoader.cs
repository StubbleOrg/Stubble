// <copyright file="StringLoader.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes.Loaders
{
    /// <summary>
    /// A noop loader
    /// </summary>
    public sealed class StringLoader : IStubbleLoader
    {
        /// <summary>
        /// Returns the passed string the parse as a template
        /// </summary>
        /// <param name="name">The name of the template to load</param>
        /// <returns>A Mustache Template</returns>
        public string Load(string name)
        {
            return name;
        }
    }
}
