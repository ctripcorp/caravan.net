using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.ValueParser
{
    public class ListParser<T> : IValueParser<List<T>>
    {
        private IValueParser<T> valueParser;

        public ListParser(IValueParser<T> valueParser)
        {
            ParameterChecker.NotNull(valueParser, "valueParser");
            this.valueParser = valueParser;
        }

        public List<T> Parse(string input)
        {
            List<T> result;
            bool success = TryParse(input, out result);
            return success ? result : null;
        }

        public bool TryParse(string input, out List<T> result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var list = new List<T>();
            var values = input.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                T element;
                if (!valueParser.TryParse(value.Trim(), out element))
                    return false;

                list.Add(element);
            }
            if (list.Count == 0)
                return false;

            result = list;
            return true;
        }
    }
}
