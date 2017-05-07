// <copyright file="StringLoader.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Loaders
{
    /// <summary>
    /// A noop loader
    /// </summary>
    public sealed class StringLoader : IStubbleLoader
    {
        /// <inheritdoc/>
        public IStubbleLoader Clone() => new StringLoader();

        /// <summary>
        /// Returns the passed string the parse as a template
        /// </summary>
        /// <param name="name">The name of the template to load</param>
        /// <returns>A Mustache Template</returns>
        public string Load(string name) => name;

        /// <summary>
        /// Returns the passed string as the template
        /// </summary>
        /// <param name="name">The template</param>
        /// <returns>The template as a task</returns>
        public ValueTask<string> LoadAsync(string name) => new ValueTask<string>(name);
    }
}
