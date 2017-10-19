namespace Com.Ctrip.Soa.Caravan.Threading.Atomic
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    [Serializable]
    public class AtomicIntegerArray : AtomicReferenceArray<int>
    {
        public AtomicIntegerArray(int length)
            : base(length)
        {
        }

        public AtomicIntegerArray(int[] array)
            : base(array)
        {
        }

        public AtomicIntegerArray(IEnumerable<int> items)
            : base(items)
        {
        }
    }
}