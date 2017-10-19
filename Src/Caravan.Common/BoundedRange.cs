using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan
{
    public class BoundedRange<T> : Range<T>, IValueCorrector<T> where T : IComparable<T>
    {
        public BoundedRange(T minValue, T maxValue)
            : base(minValue, maxValue, true, true)
        { }

        public T Correct(T value)
        {
            if (value == null)
                return value;

            if (MinValue.CompareTo(value) > 0)
                return MinValue;

            if (MaxValue.CompareTo(value) < 0)
                return MaxValue;

            return value;
        }
    }
}
