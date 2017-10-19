using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;
using Com.Ctrip.Soa.Caravan.Logging;
using System.Threading;

namespace Com.Ctrip.Soa.Caravan.Configuration.Property
{
    /// <summary>
    /// 配置项默认实现类
    /// </summary>
    class DefaultProperty : IProperty
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DefaultProperty));

        /// <summary>
        /// 初始化类 <see cref="DefaultProperty"/> 的实例.
        /// </summary>
        /// <param name="manager">与此配置关联的配置管理器</param>
        /// <param name="key">配置项</param>
        /// <param name="config">配置信息</param>
        /// <exception cref="System.ArgumentNullException">manager
        /// or
        /// config</exception>
        /// <exception cref="System.ArgumentNullException">config</exception>
        public DefaultProperty(IConfigurationManager manager, string key, PropertyConfig config)
        {
            ParameterChecker.NotNull(manager, "manager");
            ParameterChecker.NotNullOrWhiteSpace(key, "key");
            ParameterChecker.NotNull(config, "config");

            this.Manager = manager;
            this.Key = key;
            this.Config = config;
            this.Refresh();
        }

        /// <summary>
        /// 获取与此配置关联的配置管理器
        /// </summary>
        protected IConfigurationManager Manager { get; private set; }

        private volatile string value;

        /// <summary>
        /// 配置信息
        /// </summary>
        public PropertyConfig Config { get; private set; }

        /// <summary>
        /// 配置键
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 配置值
        /// </summary>
        public string Value
        {
            get
            {
                if (!Config.UseCache)
                    Refresh();

                return value;
            }
        }

        /// <summary>
        /// 添加或删除配置值改变事件
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> OnChange;

        /// <summary>
        /// 手动刷新配置值
        /// </summary>
        public virtual void Refresh()
        {
            var newValue = Manager.GetPropertyValue(Key, null);
            if (string.IsNullOrWhiteSpace(newValue))
                newValue = Config.DefaultValue;

            var oldValue = value;
            if (string.Equals(oldValue, newValue))
                return;

            value = newValue;
            RaisePropertyChangeEvent(oldValue, newValue);
        }

        private void RaisePropertyChangeEvent(string oldValue, string newValue)
        {
            if (OnChange != null)
            {
                var @event = new PropertyChangedEventArgs(Key, oldValue, newValue);
                try
                {
                    OnChange(this, @event);
                }
                catch (Exception ex)
                {
                    Logger.Error("Error occurred while raising property change event", ex);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");
            builder.AppendFormat("\"Key\":\"{0}\",", Key);
            builder.AppendFormat("\"Value\":\"{0}\",", Value);
            builder.AppendFormat("\"Config\":\"{0}\"", Config.ToString());
            builder.Append('}');
            return builder.ToString();
        }
    }
}
