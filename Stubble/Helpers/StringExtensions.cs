// <copyright file="StringExtensions.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Helpers
{
    internal static class StringExtensions
    {
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
