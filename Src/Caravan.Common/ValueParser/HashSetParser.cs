using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.ValueParser
{
    public class HashSetParser<T> : IValueParser<HashSet<T>>
    {
        private IValueParser<T> valueParser;

        public HashSetParser(IValueParser<T> valueParser)
        {
            ParameterChecker.NotNull(valueParser, "valueParser");
            this.valueParser = valueParser;
        }

        public HashSet<T> Parse(string input)
        {
            HashSet<T> result;
            bool success = TryParse(input, out result);
            return success ? result : null;
        }

        public bool TryParse(string input, out HashSet<T> result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var hashSet = new HashSet<T>();
            var values = input.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                T element;
                if (!valueParser.TryParse(value.Trim(), out element))
                    return false;

                hashSet.Add(element);
            }
            if (hashSet.Count == 0)
                return false;

            result = hashSet;
            return true;
        }
    }
}
