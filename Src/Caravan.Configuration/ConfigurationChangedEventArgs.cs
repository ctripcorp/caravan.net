using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// ConfigurationSourceChangedEventArgs 类
    /// </summary>
    public class ConfigurationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 获取发生了变化的配置项
        /// </summary>
        /// <value>The changed properties.</value>
        public PropertyChangedEventArgs[] ChangedProperties { get; private set; }

        /// <summary>
        /// 初始化 <see cref="ConfigurationChangedEventArgs"/> 类的实例.
        /// </summary>
        /// <param name="changedProperties">发生了变化的配置项</param>
        public ConfigurationChangedEventArgs(IEnumerable<PropertyChangedEventArgs> changedProperties)
        {
            this.ChangedProperties = changedProperties.ToArray();
        }
    }
}
