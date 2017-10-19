using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.ValueCorrector
{
    public class UpperBoundCorrector<T> : ValueCorrectorBase<T> where T : IComparable<T>
    {
        public T Max { get; private set; }

        public UpperBoundCorrector(T max)
            : this(null, max)
        { }

        public UpperBoundCorrector(IValueCorrector<T> innerValueCorrector, T max)
            : base(innerValueCorrector)
        {
            Max = max;
        }

        protected override T CorrectValue(T value)
        {
            if (value == null || Max == null)
                return value;

            if (value.CompareTo(Max) > 0)
                return Max;

            return value;
        }
    }

    public class NullableUpperBoundCorrector<T> : ValueCorrectorBase<T?> where T : struct, IComparable<T>
    {
        public T? Max { get; private set; }

        public NullableUpperBoundCorrector(T? max)
            : this(null, max)
        { }

        public NullableUpperBoundCorrector(IValueCorrector<T?> innerValueCorrector, T? max)
            : base(innerValueCorrector)
        {
            Max = max;
        }

        protected override T? CorrectValue(T? value)
        {
            if (value == null || Max == null)
                return value;

            if (value.Value.CompareTo(Max.Value) > 0)
                return Max;

            return value;
        }
    }
}
