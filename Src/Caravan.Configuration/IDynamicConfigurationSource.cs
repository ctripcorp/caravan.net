using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// IDynamicConfigurationSource 接口
    /// </summary>
    public interface IDynamicConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// 配置源配置改变事件
        /// </summary>
        event EventHandler<ConfigurationChangedEventArgs> OnChange;
    }
}
