using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan
{
    public class ObjectWrapper<T>
    {
        public ObjectWrapper()
        {
        }

        public ObjectWrapper(T value)
        {
            this.Value = value;
        }

        public T Value { get; set; }

        public static implicit operator T(ObjectWrapper<T> value)
        {
            if (value == null)
                return default(T);
            return value.Value;
        }

        public static implicit operator ObjectWrapper<T>(T value)
        {
            return new ObjectWrapper<T>(value);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            if (obj is T)
                return object.Equals(this.Value, (T)obj);

            var that = obj as ObjectWrapper<T>;
            if (that == null)
                return false;

            return object.Equals(this.Value, that.Value);
        }

        public override int GetHashCode()
        {
            return Value == null ? 0 : Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value == null ? "" : Value.ToString();
        }
    }
}
