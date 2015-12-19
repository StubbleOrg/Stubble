// <copyright file="LimitedSizeConcurrentDictionary.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Stubble.Core.Classes
{
    internal class LimitedSizeConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public LimitedSizeConcurrentDictionary(int maxSize)
            : base(Environment.ProcessorCount * 2, maxSize)
        {
            MaxSize = maxSize;
        }

        private int MaxSize { get; }

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
