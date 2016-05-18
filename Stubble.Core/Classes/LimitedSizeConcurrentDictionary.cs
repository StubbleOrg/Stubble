// <copyright file="LimitedSizeConcurrentDictionary.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Stubble.Core.Classes
{
    /// <summary>
    /// Represents a generic concurent dictionary which has a limited size
    /// </summary>
    /// <typeparam name="TKey">The key for the dictionary</typeparam>
    /// <typeparam name="TValue">The value for the dictionary</typeparam>
    internal class LimitedSizeConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedSizeConcurrentDictionary{TKey, TValue}"/> class
        /// with a max size.
        /// </summary>
        /// <param name="maxSize">The max size to set for the dictionary</param>
        public LimitedSizeConcurrentDictionary(int maxSize)
            : base(Environment.ProcessorCount * 2, maxSize)
        {
            MaxSize = maxSize;
        }

        private int MaxSize { get; }

        /// <summary>
        /// Gets or Sets a value in the concurrent dictionary. If would be over
        /// the max size then dump the first item in the dictionary
        /// </summary>
        /// <param name="key">The key to use to lookup in the dictionary</param>
        /// <returns>The looked up value</returns>
        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }

            set
            {
                if (Count >= MaxSize)
                {
                    DumpFirstItem();
                }

                base[key] = value;
            }
        }

        private void DumpFirstItem()
        {
            TValue output;
            TryRemove(this.First().Key, out output);
        }
    }
}
