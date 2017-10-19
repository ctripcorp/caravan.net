using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric.Null
{
    public class NullEventMetricManager : IEventMetricManager
    {
        public static NullEventMetricManager Instance { get; private set; }

        private static String NullManagerId = "null_event_metric_manager";

        static NullEventMetricManager()
        {
            Instance = new NullEventMetricManager();
        }

        private NullEventMetricManager() 
        {
            Metrics = new IEventMetric[0];
            Config = new MetricManagerConfig<IEventMetric>(NullEventMetricReporter.Instance);
        }

        public string ManagerId
        {
            get { return NullManagerId; }
        }

        public IEnumerable<IEventMetric> Metrics { get; private set; }

        public MetricManagerConfig<IEventMetric> Config { get; private set; }

        public IEventMetric GetMetric(string metricId, MetricConfig metricConfig)
        {
            return NullEventMetric.Instance;
        }
    }
}
