using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// AppSettingsConfiguration 类，从 App.config 或 Web.Config 的 appSettings 结点读取配置
    /// </summary>
    internal class AppSettingsConfiguration : IConfiguration
    {
        /// <summary>
        /// 获取指定配置键的配置
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>配置键的字符串配置值</returns>
        public string GetPropertyValue(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public string this[string key] { get { return GetPropertyValue(key); } }
    }
}
