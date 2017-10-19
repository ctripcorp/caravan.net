using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.Configuration;

namespace Com.Ctrip.Soa.Caravan.ValueCorrector
{
    [TestClass]
    public class LowerBoundCorrectorTest : TestBase
    {
        [TestMethod]
        public void NotCorrectValidValue()
        {
            string ageKey = "age";
            int ageValue = 25;
            int ageMinValue = 0;
            int defaultValue = 18;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[ageKey] = ageValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> ageProperty = manager.GetProperty<int>(ageKey, defaultValue, new LowerBoundCorrector<int>(ageMinValue));
            Assert.AreEqual(ageValue, ageProperty.Value);
        }

        [TestMethod]
        public void NotCorrectValidNullableValue()
        {
            string ageKey = "age";
            int? ageValue = 25;
            int? ageMinValue = 0;
            int? defaultValue = 18;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[ageKey] = ageValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> ageProperty = manager.GetProperty<int?>(ageKey, defaultValue, new NullableLowerBoundCorrector<int>(ageMinValue));
            Assert.AreEqual(ageValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectInvalidValue()
        {
            string ageKey = "age";
            int ageValue = -1;
            int ageMinValue = 0;
            int defaultValue = 18;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[ageKey] = ageValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> ageProperty = manager.GetProperty<int>(ageKey, defaultValue, new LowerBoundCorrector<int>(ageMinValue));
            Assert.AreEqual(ageMinValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectInvalidNullableValue()
        {
            string ageKey = "age";
            int? ageValue = -1;
            int? ageMinValue = 0;
            int? defaultValue = 18;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[ageKey] = ageValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> ageProperty = manager.GetProperty<int?>(ageKey, defaultValue, new NullableLowerBoundCorrector<int>(ageMinValue));
            Assert.AreEqual(ageMinValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectDefaultValue()
        {
            string ageKey = "age";
            int ageMinValue = 0;
            int defaultValue = -1;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> ageProperty = manager.GetProperty<int>(ageKey, defaultValue, new LowerBoundCorrector<int>(ageMinValue));
            Assert.AreEqual(ageMinValue, ageProperty.Value);
        }
        
        [TestMethod]
        public void CorrectDefaultNullableValue()
        {
            string ageKey = "age";
            int? ageMinValue = 0;
            int? defaultValue = -1;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> ageProperty = manager.GetProperty<int?>(ageKey, defaultValue, new NullableLowerBoundCorrector<int>(ageMinValue));
            Assert.AreEqual(ageMinValue, ageProperty.Value);
        }
    }
}
