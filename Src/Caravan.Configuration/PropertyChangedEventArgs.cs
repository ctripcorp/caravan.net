using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    /// <summary>
    /// 配置值改变事件参数类
    /// </summary>
    public class PropertyChangedEventArgs : PropertyChangedEventArgs<string>
    {
        internal PropertyChangedEventArgs(string key, string oldValue, string newValue)
            : base(key, oldValue, newValue, DateTime.Now)
        { }

        internal PropertyChangedEventArgs(string key, string oldValue, string newValue, DateTime changedTime)
            : base(key, oldValue, newValue, changedTime)
        { }
    }

    public class PropertyChangedEventArgs<T> : EventArgs
    {
        private string _description;

        /// <summary>
        /// 配置项
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 配置值改变之前的值
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        ///  配置值改变之后的值
        /// </summary>
        public T NewValue { get; private set; }

        /// <summary>
        /// 配置值改变的时间
        /// </summary>
        public DateTime ChangedTime { get; private set; }

        internal PropertyChangedEventArgs(string key, T oldValue, T newValue)
            : this(key, oldValue, newValue, DateTime.Now)
        { }

        internal PropertyChangedEventArgs(string key, T oldValue, T newValue, DateTime changedTime)
        {
            this.Key = key;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.ChangedTime = changedTime;
        }

        public override string ToString()
        {
            if (_description == null)
                _description = string.Format("{0} changed from {1} to {2} at {3}", Key, OldValue, NewValue, ChangedTime);
            return _description;
        }
    }
}
