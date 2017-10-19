using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.ValueCorrector
{
    public class LowerBoundCorrector<T> : ValueCorrectorBase<T> where T : IComparable<T>
    {
        public T Min { get; private set; }

        public LowerBoundCorrector(T min)
            : this(null, min)
        { }

        public LowerBoundCorrector(IValueCorrector<T> innerValueCorrector, T min)
            : base(innerValueCorrector)
        {
            Min = min;
        }

        protected override T CorrectValue(T value)
        {
            if (value == null || Min == null)
                return value;

            if (value.CompareTo(Min) < 0)
                return Min;

            return value;
        }
    }

    public class NullableLowerBoundCorrector<T> : ValueCorrectorBase<T?> where T : struct, IComparable<T>
    {
        public T? Min { get; private set; }

        public NullableLowerBoundCorrector(T? min)
            : this(null, min)
        { }

        public NullableLowerBoundCorrector(IValueCorrector<T?> innerValueCorrector, T? min)
            : base(innerValueCorrector)
        {
            Min = min;
        }

        protected override T? CorrectValue(T? value)
        {
            if (value == null || Min == null)
                return value;

            if (value.Value.CompareTo(Min.Value) < 0)
                return Min;

            return value;
        }
    }
}
