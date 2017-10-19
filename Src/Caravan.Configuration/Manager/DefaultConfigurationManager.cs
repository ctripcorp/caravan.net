using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Configuration.Property;
using Com.Ctrip.Soa.Caravan.Extensions;
using Com.Ctrip.Soa.Caravan.Configuration.Source;
using Com.Ctrip.Soa.Caravan.Utility;
using Com.Ctrip.Soa.Caravan.ValueParser;

namespace Com.Ctrip.Soa.Caravan.Configuration.Manager
{
    /// <summary>
    /// DefaultConfigurationManager 类
    /// </summary>
    class DefaultConfigurationManager : IConfigurationManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DefaultConfigurationManager));

        private void RegisterBuildInValueParser()
        {
            Register(BoolParser.Instance);
            RegisterNullable(BoolParser.Instance);
            Register(CharParser.Instance);
            RegisterNullable(CharParser.Instance);
            Register(ByteParser.Instance);
            RegisterNullable(ByteParser.Instance);
            Register(SByteParser.Instance);
            RegisterNullable(SByteParser.Instance);
            Register(ShortParser.Instance);
            RegisterNullable(ShortParser.Instance);
            Register(UShortParser.Instance);
            RegisterNullable(UShortParser.Instance);
            Register(IntParser.Instance);
            RegisterNullable(IntParser.Instance);
            Register(UIntParser.Instance);
            RegisterNullable(UIntParser.Instance);
            Register(LongParser.Instance);
            RegisterNullable(LongParser.Instance);
            Register(ULongParser.Instance);
            RegisterNullable(ULongParser.Instance);
            Register(FloatParser.Instance);
            RegisterNullable(FloatParser.Instance);
            Register(DoubleParser.Instance);
            RegisterNullable(DoubleParser.Instance);
            Register(DecimalParser.Instance);
            RegisterNullable(DecimalParser.Instance);
            Register(DateTimeParser.Instance);
            RegisterNullable(DateTimeParser.Instance);
            Register(GuidParser.Instance);
            RegisterNullable(GuidParser.Instance);

            Register(VersionParser.Instance);
            Register(StringParser.Instance);
        }

        /// <summary>
        /// 配置字符串值改变事件
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> OnPropertyChange;

        /// <summary>
        /// 获取配置源列表
        /// </summary>
        protected List<IConfigurationSource> ConfigurationSources { get; private set; }

        /// <summary>
        /// 获取配置项缓存
        /// </summary>
        protected ConcurrentDictionary<string, IProperty> PropertyCache { get; private set; }

        protected ConcurrentDictionary<Type, object> ValueParserCache { get; private set; }

        /// <summary>
        /// 初始化 <see cref="DefaultConfigurationManager"/> 类的实例.
        /// </summary>
        /// <param name="sources">配置源集合</param>
        public DefaultConfigurationManager(IEnumerable<IConfigurationSource> sources)
        {
            PropertyCache = new ConcurrentDictionary<string, IProperty>();
            ValueParserCache = new ConcurrentDictionary<Type, object>();
            ConfigurationSources = new List<IConfigurationSource>();
            RegisterBuildInValueParser();

            if (sources == null)
                return;

            foreach (var source in sources)
            {
                if (source == null || source.Configuration == null)
                    continue;

                ConfigurationSources.Add(source);
            }

            ConfigurationSources.Sort(new ConfigurationSourceComparer());
            ConfigurationSources.OfType<IDynamicConfigurationSource>().Foreach(source => source.OnChange += OnConfigurationSourceChanged);
        }

        /// <summary>
        /// 处理 <see cref="E:ConfigurationSourceChanged" /> 事件.
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">包含事件信息的 <see cref="ConfigurationChangedEventArgs"/> 类的实例</param>
        protected void OnConfigurationSourceChanged(object sender, ConfigurationChangedEventArgs e)
        {
            e.ChangedProperties.Foreach(item =>
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                    return;

                IProperty property;
                if (PropertyCache.TryGetValue(item.Key, out property))
                {
                    property.Refresh();
                }

                if (OnPropertyChange != null && !string.Equals(item.OldValue, item.NewValue))
                    OnPropertyChange(this, new PropertyChangedEventArgs(item.Key, item.OldValue, item.NewValue, item.ChangedTime));
            });
        }

        /// <summary>
        /// 获取指定配置键的配置项的字符串值
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置项的字符串值</returns>
        public string GetPropertyValue(string key, string defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            foreach (var source in ConfigurationSources)
            {
                string value = source.Configuration.GetPropertyValue(key);
                if (value != null)
                {
                    Logger.Info(string.Format("The {0}={1} has been found in source: {2}.", key, value, source.SourceId));
                    return string.IsNullOrWhiteSpace(value) ? null : value;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 创建配置项实例
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="config">配置的参数</param>
        protected IProperty NewProperty(string key, PropertyConfig config)
        {
            return new DefaultProperty(this, key, config);
        }

        /// <summary>
        /// 创建强类型的配置项实例
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="config">配置的参数</param>
        protected IProperty<T> NewProperty<T>(string key, PropertyConfig<T> config)
        {
            if (config.ValueParser == null)
                config.ValueParser = GetValueParser<T>();
            return new DefaultProperty<T>(this, key, config);
        }

        /// <summary>
        /// 获取配置源集合
        /// </summary>
        public IEnumerable<IConfigurationSource> Sources
        {
            get { return ConfigurationSources; }
        }

        /// <summary>
        /// 获取配置项集合
        /// </summary>
        public IEnumerable<string> Keys
        {
            get { return PropertyCache.Keys; }
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="config">配置的参数</param>
        /// <returns>配置项</returns>
        public IProperty GetProperty(string key, PropertyConfig config)
        {
            ParameterChecker.NotNullOrWhiteSpace(key, "key");
            return PropertyCache.GetOrAdd(key, NewProperty(key, config));
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="config">配置的参数</param>
        /// <returns>配置项</returns>
        public IProperty<T> GetProperty<T>(string key, PropertyConfig<T> config)
        {
            ParameterChecker.NotNullOrWhiteSpace(key, "key");
            return (IProperty<T>)PropertyCache.GetOrAdd(key, NewProperty<T>(key, config));
        }

        /// <summary>
        /// 为特定类型注册把字符串值转换为配置值的委托
        /// </summary>
        /// <typeparam name="T">配置值类型</typeparam>
        /// <param name="valueParser">把字符串值转换为配置值的委托</param>
        /// <exception cref="System.ArgumentNullException">valueParser</exception>
        public void Register<T>(IValueParser<T> valueParser)
        {
            ParameterChecker.NotNull(valueParser, "valueParser");
            ValueParserCache[typeof(T)] = valueParser;
        }

        private void RegisterNullable<T>(IValueParser<T> valueParser) where T : struct
        {
            Register(new NullableParser<T>(valueParser));
        }

        private IValueParser<T> GetValueParser<T>()
        {
            var type = typeof(T);

            object valueParser;
            if (ValueParserCache.TryGetValue(type, out valueParser))
                return (IValueParser<T>)valueParser;

            return null;
        }
    }
}
