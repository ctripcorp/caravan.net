using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;
using Com.Ctrip.Soa.Caravan.ValueCorrector;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// IConfigurationManager 扩展类
    /// </summary>
    public static class IConfigurationManagerExtensions
    {
        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <returns>配置项</returns>
        public static IProperty GetProperty(this IConfigurationManager manager, string key)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty(key, new PropertyConfig());
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>配置项</returns>
        public static IProperty GetProperty(this IConfigurationManager manager, string key, string defaultValue)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty(key, new PropertyConfig(defaultValue));
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>IProperty.</returns>
        public static IProperty GetProperty(this IConfigurationManager manager, string key, string defaultValue, bool useCache)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty(key, new PropertyConfig(defaultValue, useCache));
        }


        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>());
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue));
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue, bool useCache)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue, useCache));
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="valueParser">值转换器</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue, IValueParser<T> valueParser)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue, valueParser));
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="valueParser">值转换器</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue, bool useCache, IValueParser<T> valueParser)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue, useCache, valueParser));
        }


        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="valueCorrector">值修正器</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue, IValueCorrector<T> valueCorrector)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue, valueCorrector));
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="valueCorrector">值修正器</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue, bool useCache, IValueCorrector<T> valueCorrector)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue, useCache, valueCorrector));
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="valueParser">值转换器</param>
        /// <param name="valueCorrector">值修正器</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue, IValueParser<T> valueParser, IValueCorrector<T> valueCorrector)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue, valueParser, valueCorrector));
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="valueParser">值转换器</param>
        /// <param name="valueCorrector">值修正器</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue, bool useCache, IValueParser<T> valueParser, IValueCorrector<T> valueCorrector)
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue, useCache, valueParser, valueCorrector));
        }


        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">The maximum.</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue, T min, T max) where T : IComparable<T>
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue, new RangeCorrector<T>(min, max)));
        }

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="valueParser">值转换器</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>IProperty&lt;T&gt;.</returns>
        public static IProperty<T> GetProperty<T>(this IConfigurationManager manager, string key, T defaultValue, IValueParser<T> valueParser, T min, T max) where T : IComparable<T>
        {
            ParameterChecker.NotNull(manager, "manager");
            return manager.GetProperty<T>(key, new PropertyConfig<T>(defaultValue, valueParser, new RangeCorrector<T>(min, max)));
        }
    }
}
