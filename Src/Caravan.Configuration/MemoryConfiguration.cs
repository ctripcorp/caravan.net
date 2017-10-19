using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    public class MemoryConfiguration : IMemoryConfiguration
    {
        protected ConcurrentDictionary<string, string> ConfigurationMap { get; set; }

        public virtual event EventHandler<ConfigurationChangedEventArgs> OnChange;
        
        public MemoryConfiguration()
        {
            ConfigurationMap = new ConcurrentDictionary<string, string>();
        }

        public MemoryConfiguration(IEnumerable<KeyValuePair<string, string>> collection)
        {
            ConfigurationMap = new ConcurrentDictionary<string, string>();
            foreach (var item in collection)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                    throw new ArgumentException("Configuration key can not be null or empty.");
                ConfigurationMap.TryAdd(item.Key, item.Value);
            }
        }

        public virtual string SetPropertyValue(string key, string value)
        {
            string oldValue = null;
            ConfigurationMap.AddOrUpdate(key, value, (k, v) => { oldValue = v; return value; });
            if (OnChange != null && !object.Equals(oldValue, value))
            {
                var changedProperties = new PropertyChangedEventArgs[1];
                changedProperties[0] = new PropertyChangedEventArgs(key, oldValue, value);
                OnChange(this, new ConfigurationChangedEventArgs(changedProperties));
            }
            return oldValue;
        }

        public virtual string GetPropertyValue(string key)
        {
            if (!ConfigurationMap.ContainsKey(key))
                return null;
            string value = ConfigurationMap[key];
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public virtual string this[string key]
        {
            get
            {
                return GetPropertyValue(key);
            }
            set
            {
                SetPropertyValue(key, value);
            }
        }
    }
}
