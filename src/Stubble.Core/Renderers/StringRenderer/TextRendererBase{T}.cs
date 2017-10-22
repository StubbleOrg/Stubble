// <copyright file="TextRendererBase{T}.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using System.Runtime.CompilerServices;
using Stubble.Core.Contexts;
using Stubble.Core.Imported;

namespace Stubble.Core.Renderers.StringRenderer
{
    /// <summary>
    /// A base class for a generic TextRenderer
    /// </summary>
    /// <typeparam name="T">The type of the renderer</typeparam>
    public abstract class TextRendererBase<T> : TextRendererBase
        where T : TextRendererBase<T>
    {
        private char[] buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRendererBase{T}"/> class.
        /// </summary>
        /// <param name="writer">The writer to initalize with</param>
        /// <param name="rendererPipeline">The renderer pipeline to use</param>
        /// <param name="maxDepth">The max recursion depth for the renderer</param>
        protected TextRendererBase(TextWriter writer, TokenRendererPipeline<Context> rendererPipeline, uint maxDepth)
            : base(writer, rendererPipeline, maxDepth)
        {
            buffer = new char[1024];
        }

        /// <summary>
        /// Writes the content to the writer
        /// </summary>
        /// <param name="content">The content</param>
        /// <returns>The current text renderer</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(string content)
        {
            Writer.Write(content);
            return (T)this;
        }

        /// <summary>
        /// Writes the specified slice to the writer
        /// </summary>
        /// <param name="slice">the slice</param>
        /// <returns>The renderer</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(ref StringSlice slice)
        {
            if (slice.Start > slice.End)
            {
                return (T)this;
            }

            return Write(slice.Text, slice.Start, slice.Length);
        }

        /// <summary>
        /// Writes the specified slice to the writer
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <returns>The renderer</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(StringSlice slice)
        {
            return Write(ref slice);
        }

        /// <summary>
        /// Writes the character a specified number of times to the writer.
        /// </summary>
        /// <param name="character">The character to repeat</param>
        /// <param name="repeat">The number of times to repeat the character</param>
        /// <returns>The renderer</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T Write(char character, int repeat)
        {
            for (var i = 0; i < repeat; i++)
            {
                Writer.Write(character);
            }

            return (T)this;
        }

        /// <summary>
        /// Writes the specified content to the writer
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>The renderer</returns>
        public T Write(string content, int offset, int length)
        {
            if (content == null)
            {
                return (T)this;
            }

            if (offset == 0 && content.Length == length)
            {
                Writer.Write(content);
            }
            else
            {
                if (length > buffer.Length)
                {
                    buffer = content.ToCharArray();
                    Writer.Write(buffer, offset, length);
                }
                else
                {
                    content.CopyTo(offset, buffer, 0, length);
                    Writer.Write(buffer, 0, length);
                }
            }

            return (T)this;
        }
    }
}
