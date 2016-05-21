// <copyright file="TypeHelper.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
#if NETSTANDARD1_3
using System.Reflection;
#endif

namespace Stubble.Core
{
    /// <summary>
    /// Helpers for changes relating to DotNet Core
    /// </summary>
    internal static class TypeHelper
    {
        /// <summary>
        /// Determines whether the current <see cref="Type"/> from the specified <see cref="Type"/>
        /// </summary>
        /// <param name="a">The type to compare the second type to</param>
        /// <param name="b">The type to compare to the first type</param>
        /// <returns>whether the current <see cref="Type"/> from the specified <see cref="Type"/></returns>
        public static bool IsSubclassOf(this Type a, Type b)
        {
#if NETSTANDARD1_3
            return a.GetTypeInfo().IsSubclassOf(b);
#else
            return a.IsSubclassOf(b);
#endif
        }
    }
}
