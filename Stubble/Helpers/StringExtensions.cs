// <copyright file="StringExtensions.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Helpers
{
    /// <summary>
    /// A class of static string extension methods
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Slices a string between a start and end index
        /// </summary>
        /// <param name="source">the string to slice</param>
        /// <param name="start">the start index</param>
        /// <param name="end">the end index</param>
        /// <returns>A sliced string between start and end</returns>
        internal static string Slice(this string source, int start, int end)
        {
            if (end < 0)
            {
                end = source.Length + end;
            }

            var len = end - start;
            return source.Substring(start, len);
        }
    }
}
