// <copyright file="SliceHelpers.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Dev.Imported;

namespace Stubble.Core.Helpers
{
    /// <summary>
    /// Helpers for the <see cref="StringSlice"/> class
    /// </summary>
    public static class SliceHelpers
    {
        /// <summary>
        /// Splits a slice into an IEnumerable of slices split on newlines
        /// </summary>
        /// <param name="slice">The slice to split</param>
        /// <returns>The line split into lines</returns>
        public static IEnumerable<StringSlice> SplitSliceToLines(StringSlice slice)
        {
            var sliceStart = slice.Start;
            while (!slice.IsEmpty)
            {
                if (slice.CurrentChar == '\n')
                {
                    slice.NextChar();
                    yield return new StringSlice(slice.Text, sliceStart, slice.Start - 1);
                    sliceStart = slice.Start;
                    continue;
                }

                if (slice.CurrentChar == '\r' && slice.PeekChar(1) == '\n')
                {
                    slice.Start += 2;
                    yield return new StringSlice(slice.Text, sliceStart, slice.Start - 1);
                    sliceStart = slice.Start;
                    continue;
                }

                slice.NextChar();
            }

            if (sliceStart != slice.Start)
            {
                yield return new StringSlice(slice.Text, sliceStart, slice.Start - 1);
            }
        }
    }
}
