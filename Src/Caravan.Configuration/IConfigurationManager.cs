using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// IConfigurationManage 接口
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// 获取配置项集合
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// 获取配置源集合
        /// </summary>
        IEnumerable<IConfigurationSource> Sources { get; }

        /// <summary>
        /// 配置值改变事件
        /// </summary>
        event EventHandler<PropertyChangedEventArgs> OnPropertyChange;

        /// <summary>
        /// 获取指定配置键的配置项的字符串值
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置项的字符串值</returns>
        string GetPropertyValue(string key, string defaultValue);

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="config">配置的参数</param>
        /// <returns>配置项</returns>
        IProperty GetProperty(string key, PropertyConfig config);

        /// <summary>
        /// 获取指定配置键的配置项
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="config">配置的参数</param>
        /// <returns>配置项</returns>
        IProperty<T> GetProperty<T>(string key, PropertyConfig<T> config);

        /// <summary>
        /// 为特定类型注册把字符串值转换为配置值的委托
        /// </summary>
        /// <typeparam name="T">配置值类型</typeparam>
        /// <param name="valueParser">把字符串值转换为配置值的委托</param>
        void Register<T>(IValueParser<T> valueParser);
    }
}
