using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Configuration.Manager;
using Com.Ctrip.Soa.Caravan.Configuration.Source;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    public class ObjectFactory
    {
        public static IConfigurationManager CreateDefaultConfigurationManager(params IConfigurationSource[] sources)
        {
            return new DefaultConfigurationManager(sources);
        }

        public static IConfigurationSource CreateDefaultConfigurationSource(int priority, string sourceId, IConfiguration configuration)
        {
            return new DefaultConfigurationSource(priority, sourceId, configuration);
        }

        public static IDynamicConfigurationSource CreateDefaultDynamicConfigurationSource(int priority, string sourceId, IDynamicConfiguration dynamicConfiguration)
        {
            return new DefaultDynamicConfigurationSource(priority, sourceId, dynamicConfiguration);
        }

        public static IConfiguration CreateAppSettingConfiguration()
        {
            return new AppSettingsConfiguration();
        }
    }
}
