using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Metric;
using Com.Ctrip.Soa.Caravan.Metric.Null;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    public class LoadBalancerManagerConfig
    {
        public IConfigurationManager ConfigurationManager { get; private set; }

        public IEventMetricManager EventMetricManager { get; private set; }

        public IAuditMetricManager AuditMetricManager { get; private set; }

        public IStatusMetricManager<double> StatusMetricManager { get; private set; }

        public LoadBalancerManagerConfig(IConfigurationManager ConfigurationManager)
            : this(ConfigurationManager, NullEventMetricManager.Instance, NullAuditMetricManager.Instance, NullStatusMetricManager<double>.Instance)
        {
        }

        public LoadBalancerManagerConfig(IConfigurationManager configurationManager,
            IEventMetricManager eventMetricManager,
            IAuditMetricManager auditMetricManager,
            IStatusMetricManager<double> statusMetricManager)
        {
            ParameterChecker.NotNull(configurationManager, "configurationManager");
            ParameterChecker.NotNull(eventMetricManager, "eventMetricManager");
            ParameterChecker.NotNull(auditMetricManager, "auditMetricManager");
            ParameterChecker.NotNull(statusMetricManager, "statusMetricManager");

            ConfigurationManager = configurationManager;
            EventMetricManager = eventMetricManager;
            AuditMetricManager = auditMetricManager;
            StatusMetricManager = statusMetricManager;
        }
    }
}
