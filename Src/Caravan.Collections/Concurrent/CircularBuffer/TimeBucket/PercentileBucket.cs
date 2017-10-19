using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Threading.Atomic;

namespace Com.Ctrip.Soa.Caravan.Collections.Concurrent.CircularBuffer.TimeBucket.Buffer
{
    public class PercentileBucket<T> : Bucket
    {
        private T[] _data;
        private AtomicInteger _count;

        public PercentileBucket(long timeInMilliseconds, int capacity)
            : base(timeInMilliseconds)
        {
            _data = new T[capacity];
            _count = new AtomicInteger();
        }

        public void Add(T data)
        {
            if (_data.Length == 0)
                return;

            int index = (_count.IncrementAndGet() - 1) % _data.Length;
            _data[index] = data;
        }

        public int Count
        {
            get
            {
                return Math.Min(_count.Value, _data.Length);
            }
        }

        public T this[int index]
        {
            get
            {
                return _data[index];
            }
        }
    }
}
