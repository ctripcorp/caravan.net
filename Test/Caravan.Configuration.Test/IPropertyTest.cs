using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Com.Ctrip.Soa.Caravan.Configuration.Test
{
    [TestClass]
    public class IPropertyTest : TestBase
    {
        [TestMethod]
        public void AppSettingTest()
        {
            string key = "City";
            string defaultValue = "Landon";

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);
            IProperty city = configurationManager.GetProperty(key, defaultValue);

            Assert.AreEqual(configuration.GetPropertyValue(key), city.Value);
        }

        [TestMethod]
        public void DefaultValueTest()
        {
            string key = "Planet";
            string defaultValue = "Earth";

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);
            IProperty city = configurationManager.GetProperty(key, defaultValue);

            Assert.AreEqual(defaultValue, city.Value);
        }

        [TestMethod]
        public void IntValueTest()
        {
            string key = "Zero";
            int expected = 0;
            int defaultValue = 1;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);
            IProperty<int> city = configurationManager.GetProperty(key, defaultValue);

            Assert.AreEqual(expected, city.Value);
        }

        [TestMethod]
        public void CacheValueTest()
        {
            string messageKey = "message";
            string messageValue = "Hello World 1";
            string messageNewValue = "Hello World 2";

            Dictionary<string, string> data = new Dictionary<string, string>();
            data[messageKey] = messageValue;
            DynamicConfigurationForCacheValueTest configuration = new DynamicConfigurationForCacheValueTest(data);
            IDynamicConfigurationSource configurationSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "test", configuration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);
            IProperty property = manager.GetProperty(messageKey);
            Assert.AreEqual(messageValue, property.Value);

            configuration.SetPropertyValue(messageKey, messageNewValue);
            Assert.AreEqual(messageValue, property.Value);
            Thread.Sleep(2000);
            Assert.AreEqual(messageNewValue, property.Value);
        }

        [TestMethod]
        public void NotCacheValueTest()
        {
            string messageKey = "message";
            string messageValue = "Hello World 1";
            string messageNewValue = "Hello World 2";
            
            Dictionary<string, string> data = new Dictionary<string, string>();
            data[messageKey] = messageValue;
            DynamicConfigurationForCacheValueTest configuration = new DynamicConfigurationForCacheValueTest(data);
            IDynamicConfigurationSource configurationSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "test", configuration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);
            IProperty property = manager.GetProperty(messageKey, "", false);
            Assert.AreEqual(messageValue, property.Value);

            configuration.SetPropertyValue(messageKey, messageNewValue);
            Assert.AreEqual(messageNewValue, property.Value);
            Thread.Sleep(2000);
            Assert.AreEqual(messageNewValue, property.Value);
        }

        [TestMethod]
        public void PropertyOnChangeEventTest()
        {
            string key1 = "key1";
            string key2 = "key2";
            string key3 = "key3";
            string value1 = "Hello World 1";
            string value2 = "Hello World 2";
            string value3 = "100";

            Dictionary<string, string> data = new Dictionary<string, string>();
            data[key1] = value1;
            data[key2] = value2;
            data[key3] = value3;
            MemoryConfiguration configuration = new MemoryConfiguration(data);
            IDynamicConfigurationSource configurationSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "test", configuration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);
            int managerEventExecuted = 0;
            manager.OnPropertyChange += (s, e) => managerEventExecuted++;
            
            int event1Executed = 0, event2Executed = 0, event3Executed = 0;
            IProperty property1 = manager.GetProperty(key1);
            property1.OnChange += (s, e) => event1Executed++;
            IProperty property2 = manager.GetProperty(key2);
            property2.OnChange += (s, e) => event2Executed++;
            IProperty<double> property3 = manager.GetProperty(key3, 0.0);
            property3.OnChange += (s, e) => event3Executed++;

            managerEventExecuted = event1Executed = event2Executed = event3Executed = 0;
            configuration[key1] = "test value";
            Assert.AreEqual(1, event1Executed);
            Assert.AreEqual(0, event2Executed);
            Assert.AreEqual(0, event3Executed);
            Assert.AreEqual(1, managerEventExecuted);

            managerEventExecuted = event1Executed = event2Executed = event3Executed = 0;
            configuration[key2] = "test value";
            Assert.AreEqual(0, event1Executed);
            Assert.AreEqual(1, event2Executed);
            Assert.AreEqual(0, event3Executed);
            Assert.AreEqual(1, managerEventExecuted);

            managerEventExecuted = event1Executed = event2Executed = event3Executed = 0;
            configuration[key3] = "100.0";
            Assert.AreEqual(0, event1Executed);
            Assert.AreEqual(0, event2Executed);
            Assert.AreEqual(0, event3Executed);
            Assert.AreEqual(1, managerEventExecuted);
        }

        [TestMethod]
        public void PropertyMultiChangeEventTest()
        {
            string birthKey = "BirthYear";
            string ageKey = "Age";

            MultiChangeTestConfiguration configuration = new MultiChangeTestConfiguration();
            IDynamicConfigurationSource configurationSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "MultiChangeTestConfiguration", configuration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);
            IProperty<int> birthYearProperty = manager.GetProperty<int>(birthKey);
            IProperty<int> ageProperty = manager.GetProperty<int>(ageKey);
            int birthYearEventExecuted = 0, ageEventExecuted = 0;
            birthYearProperty.OnChange += (o, e) => birthYearEventExecuted++;
            ageProperty.OnChange += (o, e) => ageEventExecuted++;

            configuration[birthKey] = "1991";
            Assert.AreEqual(1, birthYearEventExecuted);
            Assert.AreEqual(1, ageEventExecuted);
        }
    }

    class DynamicConfigurationForCacheValueTest : MemoryConfiguration
    {
        public override event EventHandler<ConfigurationChangedEventArgs> OnChange;

        public DynamicConfigurationForCacheValueTest()
            : base()
        {
        }

        public DynamicConfigurationForCacheValueTest(IEnumerable<KeyValuePair<string, string>> collection)
            : base(collection)
        {
        }

        public override string SetPropertyValue(string key, string value)
        {
            string oldValue = null;
            ConfigurationMap.AddOrUpdate(key, value, (k, v) => { oldValue = v; return value; });
            if (OnChange != null && !object.Equals(oldValue, value))
            {
                var changedProperties = new PropertyChangedEventArgs[1];
                changedProperties[0] = new PropertyChangedEventArgs(key, oldValue, value);
                Task.Factory.StartNew(() => 
                {
                    Thread.Sleep(1000);
                    OnChange(this, new ConfigurationChangedEventArgs(changedProperties));
                });
            }
            return oldValue;
        }
    }

    class MultiChangeTestConfiguration : IDynamicConfiguration
    {
        private Dictionary<string, string> data = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public event EventHandler<ConfigurationChangedEventArgs> OnChange;

        public string GetPropertyValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            if (key.Equals("Age", StringComparison.InvariantCultureIgnoreCase))
            {
                var birthYear = GetPropertyValue("BirthYear");
                if (string.IsNullOrWhiteSpace(birthYear))
                    return null;
                return (DateTime.Now.Year - int.Parse(birthYear)).ToString();
            }
            string value;
            if (data.TryGetValue(key, out value))
                return value;
            return null;
        }

        public void SetPropertyValue(string key, string value)
        {
            string oldValue = GetPropertyValue("Age");
            data[key] = value;
            if (OnChange != null)
            {
                var changedProperties = new List<PropertyChangedEventArgs>();
                changedProperties.Add(new PropertyChangedEventArgs(key, oldValue, value));
                if (key.Equals("BirthYear", StringComparison.InvariantCultureIgnoreCase))
                {
                    var oldAge = GetPropertyValue("Age");
                    var newAge = (DateTime.Now.Year - int.Parse(value)).ToString();
                    changedProperties.Add(new PropertyChangedEventArgs("Age", oldAge, value));
                }
                OnChange(this, new ConfigurationChangedEventArgs(changedProperties));
            }
        }

        public string this[string index]
        {
            get { return GetPropertyValue(index); }
            set { SetPropertyValue(index, value); }
        }
    }
}
