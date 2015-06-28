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
