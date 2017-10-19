using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.ValueCorrector;

namespace Com.Ctrip.Soa.Caravan.Configuration.ValueCorrector
{
    [TestClass]
    public class ValueCorrectorBaseTest
    {
        [TestMethod]
        public void ValueCorrectorChain_MinValue_Test()
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

            IValueCorrector<int> valueCorrector = new UpperBoundCorrector<int>(portMaxValue);
            valueCorrector = new LowerBoundCorrector<int>(valueCorrector, portMinValue);
            IProperty<int> ageProperty = manager.GetProperty<int>(portKey, defaultValue, valueCorrector);
            Assert.AreEqual(portMinValue, ageProperty.Value);
        }

        [TestMethod]
        public void ValueCorrectorChain_MaxValue_Test()
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

            IValueCorrector<int> valueCorrector = new UpperBoundCorrector<int>(portMaxValue);
            valueCorrector = new LowerBoundCorrector<int>(valueCorrector, portMinValue);
            IProperty<int> ageProperty = manager.GetProperty<int>(portKey, defaultValue, valueCorrector);
            Assert.AreEqual(portMaxValue, ageProperty.Value);
        }
    }
}
