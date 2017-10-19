using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// IMemoryConfiguration 接口
    /// </summary>
    public interface IMemoryConfiguration : IDynamicConfiguration
    {
        /// <summary>
        /// 设置配置值
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        /// <returns>原有的值，若不存在则返回 null</returns>
        string SetPropertyValue(string key, string value);

        new string this[string key] { get; set; }
    }
}
