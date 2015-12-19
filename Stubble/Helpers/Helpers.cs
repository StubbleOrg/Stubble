// <copyright file="Helpers.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Stubble.Core.Helpers
{
    internal static class Helpers
    {
        /// <summary>
        /// A way to merge IDictionaries together with the right most keys overriding the left keys.
        /// Found here: http://stackoverflow.com/questions/294138/merging-dictionaries-in-c-sharp
        /// </summary>
        /// <typeparam name="TK">The type of the key</typeparam>
        /// <typeparam name="TV">The type of the value</typeparam>
        /// <param name="me">The left dictionry to merge in to</param>
        /// <param name="others">The other dictionaries to merge left</param>
        /// <returns>Returns a new dictionary with all the dictionaries keys merged left-wise.</returns>
        internal static IDictionary<TK, TV> MergeLeft<TK, TV>(this IDictionary<TK, TV> me, params IDictionary<TK, TV>[] others)
        {
            var newMap = new Dictionary<TK, TV>(me);
            foreach (var p in new List<IDictionary<TK, TV>> { me }.Concat(others).SelectMany(src => src))
            {
                newMap[p.Key] = p.Value;
            }

            return newMap;
        }
    }
}
