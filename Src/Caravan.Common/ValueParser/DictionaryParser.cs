using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.ValueParser
{
    public class DictionaryParser<TKey, TValue> : IValueParser<Dictionary<TKey, TValue>>
    {
        private IValueParser<TKey> keyParser;
        private IValueParser<TValue> valueParser;

        public DictionaryParser(IValueParser<TKey> keyParser, IValueParser<TValue> valueParser)
        {
            ParameterChecker.NotNull(keyParser, "keyParser");
            ParameterChecker.NotNull(valueParser, "valueParser");
            this.keyParser = keyParser;
            this.valueParser = valueParser;
        }

        public Dictionary<TKey, TValue> Parse(string input)
        {
            Dictionary<TKey, TValue> result;
            bool success = TryParse(input, out result);
            return success ? result : null;
        }

        public bool TryParse(string input, out Dictionary<TKey, TValue> result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var dictionary = new Dictionary<TKey, TValue>();
            var keyValuePairs = input.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyValuePair in keyValuePairs)
            {
                if (string.IsNullOrWhiteSpace(keyValuePair))
                    continue;

                var parts = keyValuePair.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return false;
                string keyString = parts[0].Trim();
                string valueString = parts[1].Trim();
                TKey key; TValue value;
                if (!keyParser.TryParse(keyString, out key) || !valueParser.TryParse(valueString, out value))
                    return false;

                dictionary[key] = value;
            }
            if (dictionary.Count == 0)
                return false;
            result = dictionary;
            return true;
        }
    }
}
