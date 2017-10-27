// <copyright file="TypeHelper.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stubble.Compilation.Helpers
{
    /// <summary>
    /// Helpers for changes relating to DotNet Core
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Returns the T of an <see cref="IEnumerable{T}"/> type
        /// </summary>
        /// <param name="type">The generic type to get the value from</param>
        /// <returns>The internal type or null if not found</returns>
        public static Type GetElementTypeOfIEnumerable(this Type type)
        {
            // Type is Array
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            // type is IEnumerable<T>;
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments()[0];
            }

            return type.GetInterfaces()
                .Where(t => t.GetTypeInfo().IsGenericType == true && (t.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>) || t.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerator<>)))
                .Select(s => s.GetGenericArguments()[0])
                .FirstOrDefault();
        }
    }
}
