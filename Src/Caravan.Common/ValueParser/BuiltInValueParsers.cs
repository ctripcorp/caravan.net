using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.ValueParser
{
    public class StringParser : IValueParser<string>
    {
        public readonly static StringParser Instance = new StringParser();

        public string Parse(string value)
        {
            return value;
        }

        public bool TryParse(string input, out string result)
        {
            result = input;
            return true;
        }
    }

    public class BoolParser : IValueParser<bool>
    {
        public readonly static BoolParser Instance = new BoolParser();

        public bool Parse(string value)
        {
            return bool.Parse(value);
        }

        public bool TryParse(string input, out bool result)
        {
            return bool.TryParse(input, out result);
        }
    }

    public class CharParser : IValueParser<char>
    {
        public readonly static CharParser Instance = new CharParser();

        public char Parse(string value)
        {
            return char.Parse(value);
        }

        public bool TryParse(string input, out char result)
        {
            return char.TryParse(input, out result);
        }
    }

    public class ByteParser : IValueParser<byte>
    {
        public readonly static ByteParser Instance = new ByteParser();

        public byte Parse(string value)
        {
            return byte.Parse(value);
        }

        public bool TryParse(string input, out byte result)
        {
            return byte.TryParse(input, out result);
        }
    }

    public class SByteParser : IValueParser<sbyte>
    {
        public readonly static SByteParser Instance = new SByteParser();

        public sbyte Parse(string value)
        {
            return sbyte.Parse(value);
        }

        public bool TryParse(string input, out sbyte result)
        {
            return sbyte.TryParse(input, out result);
        }
    }

    public class ShortParser : IValueParser<short>
    {
        public readonly static ShortParser Instance = new ShortParser();

        public short Parse(string value)
        {
            return short.Parse(value);
        }

        public bool TryParse(string input, out short result)
        {
            return short.TryParse(input, out result);
        }
    }

    public class UShortParser : IValueParser<ushort>
    {
        public readonly static UShortParser Instance = new UShortParser();

        public ushort Parse(string value)
        {
            return ushort.Parse(value);
        }

        public bool TryParse(string input, out ushort result)
        {
            return ushort.TryParse(input, out result);
        }
    }

    public class IntParser : IValueParser<int>
    {
        public readonly static IntParser Instance = new IntParser();

        public int Parse(string value)
        {
            return int.Parse(value);
        }

        public bool TryParse(string input, out int result)
        {
            return int.TryParse(input, out result);
        }
    }

    public class UIntParser : IValueParser<uint>
    {
        public readonly static UIntParser Instance = new UIntParser();

        public uint Parse(string value)
        {
            return uint.Parse(value);
        }

        public bool TryParse(string input, out uint result)
        {
            return uint.TryParse(input, out result);
        }
    }

    public class LongParser : IValueParser<long>
    {
        public readonly static LongParser Instance = new LongParser();

        public long Parse(string value)
        {
            return long.Parse(value);
        }

        public bool TryParse(string input, out long result)
        {
            return long.TryParse(input, out result);
        }
    }

    public class ULongParser : IValueParser<ulong>
    {
        public readonly static ULongParser Instance = new ULongParser();

        public ulong Parse(string value)
        {
            return ulong.Parse(value);
        }

        public bool TryParse(string input, out ulong result)
        {
            return ulong.TryParse(input, out result);
        }
    }

    public class FloatParser : IValueParser<float>
    {
        public readonly static FloatParser Instance = new FloatParser();

        public float Parse(string value)
        {
            return float.Parse(value);
        }

        public bool TryParse(string input, out float result)
        {
            return float.TryParse(input, out result);
        }
    }

    public class DoubleParser : IValueParser<double>
    {
        public readonly static DoubleParser Instance = new DoubleParser();

        public double Parse(string value)
        {
            return double.Parse(value);
        }

        public bool TryParse(string input, out double result)
        {
            return double.TryParse(input, out result);
        }
    }

    public class DecimalParser : IValueParser<decimal>
    {
        public readonly static DecimalParser Instance = new DecimalParser();

        public decimal Parse(string value)
        {
            return decimal.Parse(value);
        }

        public bool TryParse(string input, out decimal result)
        {
            return decimal.TryParse(input, out result);
        }
    }

    public class DateTimeParser : IValueParser<DateTime>
    {
        public readonly static DateTimeParser Instance = new DateTimeParser();

        public DateTime Parse(string value)
        {
            return DateTime.Parse(value);
        }

        public bool TryParse(string input, out DateTime result)
        {
            return DateTime.TryParse(input, out result);
        }
    }

    public class GuidParser : IValueParser<Guid>
    {
        public readonly static GuidParser Instance = new GuidParser();

        public Guid Parse(string value)
        {
            return Guid.Parse(value);
        }

        public bool TryParse(string input, out Guid result)
        {
            return Guid.TryParse(input, out result);
        }
    }

    public class VersionParser : IValueParser<Version>
    {
        public readonly static VersionParser Instance = new VersionParser();

        public Version Parse(string value)
        {
            return Version.Parse(value);
        }

        public bool TryParse(string input, out Version result)
        {
            return Version.TryParse(input, out result);
        }
    }

    public class NullableParser<T> : IValueParser<T?> where T : struct
    {
        private IValueParser<T> valueParser;

        public NullableParser(IValueParser<T> valueParser)
        {
            ParameterChecker.NotNull(valueParser, "valueParser");
            this.valueParser = valueParser;
        }

        public T? Parse(string input)
        {
            return valueParser.Parse(input);
        }

        public bool TryParse(string input, out T? result)
        {
            T value;
            bool success = valueParser.TryParse(input, out value);
            result = success ? value : default(T);
            return success;
        }
    }
}
