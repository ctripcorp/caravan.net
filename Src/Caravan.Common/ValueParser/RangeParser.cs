using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;
using System.Text.RegularExpressions;

namespace Com.Ctrip.Soa.Caravan.ValueParser
{
    public class RangeParser<T> : IValueParser<Range<T>> where T : IComparable<T>
    {
        private const string IncludeMinValueGroupName = "includeMinValue";
        private const string IncludeMaxValueGroupName = "includeMaxValue";
        private const string MinValueGroupName = "minValue";
        private const string MaxValueGroupName = "maxValue";

        private static readonly Regex regex;

        static RangeParser()
        {
            var pattern = string.Format(@"
^\s*(?<{0}>\[|\()?\s*     # try match left bound
(?({0})                   # has left bound ?
  (                       # true
    (?<{1}>\d+)\s*,       # left value
    \s*(?<{2}>\d+)\s*     # right value
    (?<{3}>\]|\))         # right bound
  )
  |                       # else
  (                       # false
    (?<{1}>\d+)\s*,       # left value
    \s*(?<{2}>\d+)        # right bound
  )
)\s*$",
                IncludeMinValueGroupName, MinValueGroupName, MaxValueGroupName, IncludeMaxValueGroupName);
            regex = new Regex(pattern, RegexOptions.Compiled);
        }

        private IValueParser<T> valueParser;

        public RangeParser(IValueParser<T> valueParser)
        {
            ParameterChecker.NotNull(valueParser, "valueParser");
            this.valueParser = valueParser;
        }

        public Range<T> Parse(string input)
        {
            Range<T> result;
            bool success = TryParse(input, out result);
            return success ? result : null;
        }

        public bool TryParse(string input, out Range<T> result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            Match match = regex.Match(input);
            if (!match.Success)
                return false;

            bool includeMinValue = match.Groups[IncludeMinValueGroupName].Value == "[";
            bool includeMaxValue = match.Groups[IncludeMaxValueGroupName].Value == "]";

            string minValueCapture = match.Groups[MinValueGroupName].Value;
            string maxValueCapture = match.Groups[MaxValueGroupName].Value;
            T minValue, maxValue;
            if (!valueParser.TryParse(minValueCapture, out minValue) || !valueParser.TryParse(maxValueCapture, out maxValue))
                return false;

            result = new Range<T>(minValue, maxValue, includeMinValue, includeMaxValue);
            return true;
        }
    }
}
