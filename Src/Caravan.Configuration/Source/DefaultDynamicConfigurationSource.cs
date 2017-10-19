using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration.Source
{
    /// <summary>
    /// Class DefaultDynamicConfigurationSource.
    /// </summary>
    public class DefaultDynamicConfigurationSource : DefaultConfigurationSource, IDynamicConfigurationSource
    {
        /// <summary>
        /// 配置源配置改变事件
        /// </summary>
        public event EventHandler<ConfigurationChangedEventArgs> OnChange
        {
            add 
            {
                DynamicConfiguration.OnChange += value;
            }
            remove 
            {
                DynamicConfiguration.OnChange -= value;
            }
        }

        /// <summary>
        /// 获取或设置配置
        /// </summary>
        protected IDynamicConfiguration DynamicConfiguration 
        {
            get
            {
                return (IDynamicConfiguration)base.Configuration;
            }
            set
            {
                base.Configuration = value;
            }
        }

        /// <summary>
        /// 初始化 <see cref="DefaultDynamicConfigurationSource"/> 类的实例.
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="sourceId">标识</param>
        /// <param name="configuration">配置</param>
        public DefaultDynamicConfigurationSource(int priority, string sourceId, IDynamicConfiguration configuration)
            : base(priority, sourceId, configuration)
        {
        }
    }
}
