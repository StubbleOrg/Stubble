// <copyright file="IStubbleLoader.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Interfaces
{
    public interface IStubbleLoader
    {
        /// <summary>
        /// Loads a template with the given name.
        ///
        /// Returns null if the template is not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A Mustache Template</returns>
        string Load(string name);
    }
}
