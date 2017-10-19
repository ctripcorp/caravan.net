using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// IConfigurationSource 接口
    /// </summary>
    public interface IConfigurationSource
    {
        /// <summary>
        /// 获取配置源优先级
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 获取配置源的标识
        /// </summary>
        /// <value>The source identifier.</value>
        string SourceId { get; }

        /// <summary>
        /// 获取配置源的配置
        /// </summary>
        IConfiguration Configuration { get; }
    }
}
