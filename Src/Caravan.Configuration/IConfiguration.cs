using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// IConfiguration 接口
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// 获取指定配置键的配置
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>配置键的字符串配置值</returns>
        string GetPropertyValue(string key);

        string this[string index] { get; }
    }
}
