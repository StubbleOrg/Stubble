using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes
{
    public class LimitedSizeConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        private int MaxSize { get; set; }

        public LimitedSizeConcurrentDictionary(int maxSize)
            : base(Environment.ProcessorCount * 2, maxSize)
        {
            MaxSize = maxSize;
        }

        private void DumpFirstItem()
        {
            TValue output;
            TryRemove(this.First().Key, out output);
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            if (Count >= MaxSize)
            {
                DumpFirstItem();
            }
            return base.TryAdd(key, value);
        }

        public new TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (Count >= MaxSize)
            {
                DumpFirstItem();
            }
            return base.AddOrUpdate(key, addValueFactory, updateValueFactory);
        }

        public new TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (Count >= MaxSize)
            {
                DumpFirstItem();
            }
            return base.AddOrUpdate(key, addValue, updateValueFactory);
        }

        public new TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (Count >= MaxSize)
            {
                DumpFirstItem();
            }
            return base.GetOrAdd(key, valueFactory);
        }

        public new TValue GetOrAdd(TKey key, TValue value)
        {
            if (Count >= MaxSize)
            {
                DumpFirstItem();
            }
            return base.GetOrAdd(key, value);
        }

        public new TValue this[TKey key]
        {
            get { return base[key]; }
            set {
                if (Count >= MaxSize)
                {
                    DumpFirstItem();
                }
                base[key] = value;
            }
        }
    }
}
