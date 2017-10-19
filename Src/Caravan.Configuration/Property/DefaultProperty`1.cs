using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Configuration.Manager;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.Configuration.Property
{
    /// <summary>
    /// 配置项默认实现类
    /// </summary>
    class DefaultProperty<T> : DefaultProperty, IProperty<T>
    {
        private static ILog Logger = LogManager.GetLogger(typeof(DefaultProperty<>));

        /// <summary>
        /// 初始化类 <see cref="DefaultProperty{T}" /> 的实例.
        /// </summary>
        /// <param name="manager">与此配置关联的配置管理器</param>
        /// <param name="key">配置项</param>
        /// <param name="config">配置信息</param>
        /// <exception cref="System.ArgumentNullException">manager
        /// or
        /// config</exception>
        /// <exception cref="System.ArgumentException">key is null or empty!</exception>
        public DefaultProperty(IConfigurationManager manager, string key, PropertyConfig<T> config)
            : base(manager, key, config)
        {
        }

        private volatile ObjectWrapper<T> value;

        /// <summary>
        /// 配置信息
        /// </summary>
        public new PropertyConfig<T> Config
        {
            get
            {
                return (PropertyConfig<T>)base.Config;
            }
        }

        /// <summary>
        /// 配置值
        /// </summary>
        public new T Value 
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
        public new event EventHandler<PropertyChangedEventArgs<T>> OnChange;

        /// <summary>
        /// 手动刷新配置值
        /// </summary>
        public override void Refresh()
        {
            T newValue;
            var plainValue = Manager.GetPropertyValue(Key, null);
            if (string.IsNullOrWhiteSpace(plainValue))
                newValue = Config.DefaultValue;
            else if (Config.ValueParser == null)
            {
                newValue = Config.DefaultValue;
                Logger.Warn(string.Format("ValueParser is null! Key:{0}, Value type:{1}", Key, typeof(T).FullName));
            }
            else if (!Config.ValueParser.TryParse(plainValue, out newValue))
            {
                newValue = Config.DefaultValue;
                Logger.Warn(string.Format("Failed to parse property value! Key:{0}, String value:{1}, ValueParser:{2}", Key, plainValue, Config.ValueParser.GetType().FullName));
            }

            if (Config.ValueCorrector != null)
                newValue = Config.ValueCorrector.Correct(newValue);

            var oldValue = value;
            if (object.Equals(oldValue, newValue))
                return;

            value = newValue;
            RaisePropertyChangeEvent(oldValue, newValue);
        }

        private void RaisePropertyChangeEvent(T oldValue, T newValue)
        {
            if (OnChange != null)
            {
                var @event = new PropertyChangedEventArgs<T>(Key, oldValue, newValue);
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
