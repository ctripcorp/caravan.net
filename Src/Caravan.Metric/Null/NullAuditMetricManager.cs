using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric.Null
{
    public class NullAuditMetricManager : IAuditMetricManager
    {
        public static NullAuditMetricManager Instance { get; private set; }

        private static string NullManagerId = "null_audit_metric_manager";

        static NullAuditMetricManager()
        {
            Instance = new NullAuditMetricManager();
        }
        
        private NullAuditMetricManager()
        {
            Metrics = new IAuditMetric[0];
            Config = new MetricManagerConfig<IAuditMetric>(NullAuditMetricReporter.Instance);
        }

        public string ManagerId
        {
            get { return NullManagerId; }
        }

        public IEnumerable<IAuditMetric> Metrics { get; private set; }

        public MetricManagerConfig<IAuditMetric> Config { get; private set; }

        public IAuditMetric GetMetric(string metricId, MetricConfig metricConfig)
        {
            return NullAuditMetric.Instance;
        }
    }
}
