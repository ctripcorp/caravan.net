using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration.Source
{
    /// <summary>
    /// ConfigurationSource 类
    /// </summary>
    public class DefaultConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// 初始化 <see cref="DefaultConfigurationSource" /> 类的实例.
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="sourceId">标识</param>
        /// <param name="configuration">配置</param>
        /// <exception cref="System.ArgumentException">Argument \sourceId\ is null or empty</exception>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public DefaultConfigurationSource(int priority, string sourceId, IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(sourceId))
                throw new ArgumentException("Argument \"sourceId\" is null or empty");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            this.Priority = priority;
            this.SourceId = sourceId;
            this.Configuration = configuration;
        }

        /// <summary>
        /// 获取配置源优先级
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; protected set; }

        /// <summary>
        /// 获取配置源的标识
        /// </summary>
        /// <value>The source identifier.</value>
        public string SourceId { get; protected set; }

        /// <summary>
        /// 获取配置源的配置
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; protected set; }
    }
}
