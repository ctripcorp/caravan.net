using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan
{
    public class Range<T> where T : IComparable<T>
    {
        public T MinValue { get; private set; }

        public T MaxValue { get; private set; }

        public bool IncludeMinValue { get; private set; }

        public bool IncludeMaxValue { get; private set; }

        public Range(T minValue, T maxValue)
            : this(minValue, maxValue, true, true)
        { }

        public Range(T minValue, T maxValue, bool includeMinValue, bool includeMaxValue)
        {
            ParameterChecker.NotNull(minValue, "minValue");
            ParameterChecker.NotNull(maxValue, "maxValue");

            MinValue = minValue;
            MaxValue = maxValue;
            IncludeMinValue = includeMinValue;
            IncludeMaxValue = includeMaxValue;
        }

        public bool Contains(T value)
        {
            ParameterChecker.NotNull(value, "value");

            if (MinValue.CompareTo(value) > 0)
                return false;
            if (!IncludeMinValue && MinValue.CompareTo(value) == 0)
                return false;

            if (MaxValue.CompareTo(value) < 0)
                return false;
            if (!IncludeMaxValue && MaxValue.CompareTo(value) == 0)
                return false;

            return true;
        }
    }
}
