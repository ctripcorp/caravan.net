using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    public class PropertyConfig
    {
        public string DefaultValue { get; private set; }

        public bool UseCache { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyConfig"/> class.
        /// </summary>
        public PropertyConfig()
            : this(null, true)
        { 
        }

        public PropertyConfig(string defaultValue)
            : this(defaultValue, true)
        {
        }

        public PropertyConfig(string defaultValue, bool useCache)
        {
            UseCache = useCache;
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            return string.Format("{{\"DefaultValue\":\"{0}\",\"UseCache\":\"{1}\"}}", DefaultValue, UseCache);
        }
    }

    public class PropertyConfig<T> : PropertyConfig
    {
        public new T DefaultValue { get; private set; }

        public IValueParser<T> ValueParser { get; internal set; }

        public IValueCorrector<T> ValueCorrector { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyConfig{T}"/> class.
        /// </summary>
        public PropertyConfig()
            : this(default(T), true, null, null)
        {
 
        }
        public PropertyConfig(T defaultValue)
            : this(defaultValue, true, null, null)
        {

        }

        public PropertyConfig(T defaultValue, bool useCache)
            : this(defaultValue, useCache, null, null)
        {

        }

        public PropertyConfig(T defaultValue, IValueParser<T> valueParser)
            : this(defaultValue, true, valueParser, null)
        { 

        }

        public PropertyConfig(T defaultValue, IValueCorrector<T> valueCorrector)
            : this(defaultValue, true, null, valueCorrector)
        {

        }

        public PropertyConfig(T defaultValue, bool useCache, IValueParser<T> valueParser)
            : this(defaultValue, useCache, valueParser, null)
        {
        }

        public PropertyConfig(T defaultValue, bool useCache, IValueCorrector<T> valueCorrector)
            : this(defaultValue, useCache, null, valueCorrector)
        {
        }

        public PropertyConfig(T defaultValue, IValueParser<T> valueParser, IValueCorrector<T> valueCorrector)
            : this(defaultValue, true, valueParser, valueCorrector)
        {
        }

        public PropertyConfig(T defaultValue, bool useCache, IValueParser<T> valueParser, IValueCorrector<T> valueCorrector)
            : base(defaultValue == null ? null : defaultValue.ToString(), useCache)
        {
            DefaultValue = defaultValue;
            ValueParser = valueParser;
            ValueCorrector = valueCorrector;
        }

        public override string ToString()
        {
            return string.Format("{{\"DefaultValue\":\"{0}\",\"UseCache\":\"{1}\"}}", DefaultValue, UseCache);
        }
    }
}
