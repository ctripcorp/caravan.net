using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric.Null
{
    public class NullStatusMetricManager<T> : IStatusMetricManager<T>
    {
        public static NullStatusMetricManager<T> Instance { get; private set; }

        private static String NullManagerId = "null_status_metric_manager";

        static NullStatusMetricManager()
        {
            Instance = new NullStatusMetricManager<T>();
        }

        private NullStatusMetricManager() 
        {
            Metrics = new NullStatusMetric<T>[0];
            Config = new MetricManagerConfig<IStatusMetric<T>>(NullStatusMetricReporter<T>.Instance);
        }

        public string ManagerId
        {
            get { return NullManagerId; }
        }

        public IEnumerable<IStatusMetric<T>> Metrics { get; private set; }

        public MetricManagerConfig<IStatusMetric<T>> Config { get; private set; }
        
        public IStatusMetric<T> GetMetric(string metricId, StatusMetricConfig<T> metricConfig)
        {
            return NullStatusMetric<T>.Instance;
        }
    }
}
