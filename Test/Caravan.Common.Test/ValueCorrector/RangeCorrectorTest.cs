using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.ValueCorrector;

namespace Com.Ctrip.Soa.Caravan.Configuration.ValueCorrector
{
    [TestClass]
    public class RangeCorrectorTest : TestBase
    {
        [TestMethod]
        public void NotCorrectValidValue()
        {
            string portKey = "port";
            int portValue = 1023;
            int portMinValue = 1;
            int portMaxValue = 65535;
            int defaultValue = 0;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[portKey] = portValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> sizeProperty = manager.GetProperty<int>(portKey, defaultValue, new RangeCorrector<int>(portMinValue, portMaxValue));
            Assert.AreEqual(portValue, sizeProperty.Value);
        }

        [TestMethod]
        public void NotCorrectValidNullableValue()
        {
            string portKey = "port";
            int? portValue = 1023;
            int? portMinValue = 1;
            int? portMaxValue = 65535;
            int? defaultValue = 0;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[portKey] = portValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> sizeProperty = manager.GetProperty<int?>(portKey, defaultValue, new NullableRangeCorrector<int>(portMinValue, portMaxValue));
            Assert.AreEqual(portValue, sizeProperty.Value);
        }

        [TestMethod]
        public void CorrectInvalidValue_MinValue()
        {
            string portKey = "port";
            int portValue = 0;
            int portMinValue = 1;
            int portMaxValue = 65535;
            int defaultValue = 12345;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[portKey] = portValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> ageProperty = manager.GetProperty<int>(portKey, defaultValue, new RangeCorrector<int>(portMinValue, portMaxValue));
            Assert.AreEqual(portMinValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectInvalidValue_NullableMinValue()
        {
            string portKey = "port";
            int? portValue = 0;
            int? portMinValue = 1;
            int? portMaxValue = 65535;
            int? defaultValue = 12345;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[portKey] = portValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> ageProperty = manager.GetProperty<int?>(portKey, defaultValue, new NullableRangeCorrector<int>(portMinValue, portMaxValue));
            Assert.AreEqual(portMinValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectInvalidValue_MaxValue()
        {
            string portKey = "port";
            int portValue = 65536;
            int portMinValue = 1;
            int portMaxValue = 65535;
            int defaultValue = 12345;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[portKey] = portValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> ageProperty = manager.GetProperty<int>(portKey, defaultValue, new RangeCorrector<int>(portMinValue, portMaxValue));
            Assert.AreEqual(portMaxValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectInvalidValue_NullableMaxValue()
        {
            string portKey = "port";
            int? portValue = 65536;
            int? portMinValue = 1;
            int? portMaxValue = 65535;
            int? defaultValue = 12345;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[portKey] = portValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> ageProperty = manager.GetProperty<int?>(portKey, defaultValue, new NullableRangeCorrector<int>(portMinValue, portMaxValue));
            Assert.AreEqual(portMaxValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectDefaultValue()
        {
            string portKey = "port";
            int portMinValue = 1;
            int portMaxValue = 65535;
            int defaultValue = 0;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> ageProperty = manager.GetProperty<int>(portKey, defaultValue, new RangeCorrector<int>(portMinValue, portMaxValue));
            Assert.AreEqual(portMinValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectDefaultNullableValue()
        {
            string portKey = "port";
            int? portMinValue = 1;
            int? portMaxValue = 65535;
            int? defaultValue = 0;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> ageProperty = manager.GetProperty<int?>(portKey, defaultValue, new NullableRangeCorrector<int>(portMinValue, portMaxValue));
            Assert.AreEqual(portMinValue, ageProperty.Value);
        }
    }
}
