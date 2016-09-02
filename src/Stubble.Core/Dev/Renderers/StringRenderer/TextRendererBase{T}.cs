// <copyright file="TextRendererBase{T}.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;

namespace Stubble.Core.Dev.Renderers.StringRenderer
{
    /// <summary>
    /// A base class for a generic TextRenderer
    /// </summary>
    /// <typeparam name="T">The type of the renderer</typeparam>
    public abstract class TextRendererBase<T> : TextRendererBase
        where T : TextRendererBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextRendererBase{T}"/> class.
        /// </summary>
        /// <param name="writer">The writer to initalize with</param>
        protected TextRendererBase(TextWriter writer)
            : base(writer)
        {
        }

        /// <summary>
        /// Writes the content to the writer
        /// </summary>
        /// <param name="content">The content</param>
        /// <returns>The current text renderer</returns>
        public T Write(string content)
        {
            Writer.Write(content);
            return (T)this;
        }
    }
}
