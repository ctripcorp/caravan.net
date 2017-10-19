using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.ValueCorrector;

namespace Com.Ctrip.Soa.Caravan.Configuration.ValueCorrector
{
    [TestClass]
    public class UpperBoundCorrectorTest : TestBase
    {
        [TestMethod]
        public void NotCorrectValidValue()
        {
            string sizeKey = "size";
            int sizeValue = 25;
            int sizeMaxValue = 100;
            int defaultValue = 0;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[sizeKey] = sizeValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> sizeProperty = manager.GetProperty<int>(sizeKey, defaultValue, new UpperBoundCorrector<int>(sizeMaxValue));
            Assert.AreEqual(sizeValue, sizeProperty.Value);
        }
        
        [TestMethod]
        public void NotCorrectValidNullableValue()
        {
            string sizeKey = "size";
            int? sizeValue = 25;
            int? sizeMaxValue = 100;
            int? defaultValue = 0;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[sizeKey] = sizeValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> sizeProperty = manager.GetProperty<int?>(sizeKey, defaultValue, new NullableUpperBoundCorrector<int>(sizeMaxValue));
            Assert.AreEqual(sizeValue, sizeProperty.Value);
        }

        [TestMethod]
        public void CorrectInvalidValue()
        {
            string sizeKey = "age";
            int sizeValue = 101;
            int sizeMaxValue = 100;
            int defaultValue = 0;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[sizeKey] = sizeValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> ageProperty = manager.GetProperty<int>(sizeKey, defaultValue, new UpperBoundCorrector<int>(sizeMaxValue));
            Assert.AreEqual(sizeMaxValue, ageProperty.Value);
        }
        
        [TestMethod]
        public void CorrectInvalidNullableValue()
        {
            string sizeKey = "age";
            int? sizeValue = 101;
            int? sizeMaxValue = 100;
            int? defaultValue = 0;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            memoryConfiguration[sizeKey] = sizeValue.ToString();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> ageProperty = manager.GetProperty<int?>(sizeKey, defaultValue, new NullableUpperBoundCorrector<int>(sizeMaxValue));
            Assert.AreEqual(sizeMaxValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectDefaultValue()
        {
            string sizeKey = "age";
            int sizeMaxValue = 100;
            int defaultValue = 101;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int> ageProperty = manager.GetProperty<int>(sizeKey, defaultValue, new UpperBoundCorrector<int>(sizeMaxValue));
            Assert.AreEqual(sizeMaxValue, ageProperty.Value);
        }

        [TestMethod]
        public void CorrectDefaultNullableValue()
        {
            string sizeKey = "age";
            int? sizeMaxValue = 100;
            int? defaultValue = 101;
            MemoryConfiguration memoryConfiguration = new MemoryConfiguration();
            IDynamicConfigurationSource dynamicSource = ObjectFactory.CreateDefaultDynamicConfigurationSource(0, "dynamic", memoryConfiguration);
            IConfigurationManager manager = ObjectFactory.CreateDefaultConfigurationManager(dynamicSource);

            IProperty<int?> ageProperty = manager.GetProperty<int?>(sizeKey, defaultValue, new NullableUpperBoundCorrector<int>(sizeMaxValue));
            Assert.AreEqual(sizeMaxValue, ageProperty.Value);
        }
    }
}
