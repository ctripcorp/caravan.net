using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.ValueCorrector
{
    public class RangeCorrector<T> : ValueCorrectorBase<T> where T : IComparable<T>
    {
        public T Min { get; private set; }

        public T Max { get; private set; }

        public RangeCorrector(T min, T max)
            : this(null, min, max)
        { }

        public RangeCorrector(IValueCorrector<T> innerValueCorrector, T min, T max)
            : base(innerValueCorrector)
        {
            Min = min;
            Max = max;
        }

        protected override T CorrectValue(T value)
        {
            if (value == null)
                return value;

            if (Min != null && value.CompareTo(Min) < 0)
                return Min;
            if (Max != null && value.CompareTo(Max) > 0)
                return Max;

            return value;
        }
    }

    public class NullableRangeCorrector<T> : ValueCorrectorBase<T?> where T : struct, IComparable<T>
    {
        public T? Min { get; private set; }

        public T? Max { get; private set; }

        public NullableRangeCorrector(T? min, T? max)
            : this(null, min, max)
        { }

        public NullableRangeCorrector(IValueCorrector<T?> innerValueCorrector, T? min, T? max)
            : base(innerValueCorrector)
        {
            Min = min;
            Max = max;
        }

        protected override T? CorrectValue(T? value)
        {
            if (value == null)
                return value;

            if (Min != null && value.Value.CompareTo(Min.Value) < 0)
                return Min;
            if (Max != null && value.Value.CompareTo(Max.Value) > 0)
                return Max;

            return value;
        }
    }
}
