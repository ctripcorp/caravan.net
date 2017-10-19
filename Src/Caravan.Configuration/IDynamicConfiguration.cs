using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// IDynamicConfiguration 接口
    /// </summary>
    public interface IDynamicConfiguration : IConfiguration
    {
        /// <summary>
        /// 配置改变
        /// </summary>
        event EventHandler<ConfigurationChangedEventArgs> OnChange;
    }
}
