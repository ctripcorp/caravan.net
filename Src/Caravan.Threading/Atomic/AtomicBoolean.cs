#pragma warning disable 420

namespace Com.Ctrip.Soa.Caravan.Threading.Atomic
{
    using System;
    using System.Globalization;
    using System.Threading;

    [Serializable]
    public class AtomicBoolean : IFormattable
    {
        private volatile int booleanValue;

        public bool Value
        {
            get
            {
                return this.booleanValue != 0;
            }
            set
            {
                this.booleanValue = value ? 1 : 0;
            }
        }

        public AtomicBoolean()
            : this(false)
        {
        }

        public AtomicBoolean(bool initialValue)
        {
            Value = initialValue;
        }

        public bool CompareAndSet(bool expect, bool update)
        {
            int expectedIntValue = expect ? 1 : 0;
            int newIntValue = update ? 1 : 0;
            return Interlocked.CompareExchange(ref this.booleanValue, newIntValue, expectedIntValue) == expectedIntValue;
        }

        public bool GetAndSet(bool newValue)
        {
            return Interlocked.Exchange(ref this.booleanValue, newValue ? 1 : 0) != 0;
        }

        public bool WeakCompareAndSet(bool expect, bool update)
        {
            return CompareAndSet(expect, update);
        }

        public override bool Equals(object obj)
        {
            return obj as AtomicBoolean == this;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return Value.ToString(formatProvider);
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Value.ToString(formatProvider);
        }

        public static bool operator ==(AtomicBoolean left, AtomicBoolean right)
        {
            if (Object.ReferenceEquals(left, null) && Object.ReferenceEquals(right, null))
                return true;
            if (Object.ReferenceEquals(left, null) || Object.ReferenceEquals(right, null))
                return false;

            return left.Value == right.Value;
        }

        public static bool operator !=(AtomicBoolean left, AtomicBoolean right)
        {
            return !(left == right);
        }

        public static implicit operator bool(AtomicBoolean atomic)
        {
            if (Object.ReferenceEquals(atomic, null))
            {
                return false;
            }
            else
            {
                return atomic.Value;
            }
        }

        public static implicit operator AtomicBoolean(bool value)
        {
            return new AtomicBoolean(value);
        }
    }
}

#pragma warning disable 420