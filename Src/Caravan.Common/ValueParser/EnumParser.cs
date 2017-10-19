using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.ValueParser
{
    public class EnumParser<T> : IValueParser<T> where T : struct
    {
        public EnumParser()
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException(type.FullName + " is not an enumeration type!");
        }

        public T Parse(string input)
        {
            return (T)Enum.Parse(typeof(T), input);
        }

        public bool TryParse(string input, out T result)
        {
            return Enum.TryParse<T>(input, out result);
        }
    }
}
