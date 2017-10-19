using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric.Null
{
    public class NullStatusMetric<T> : IStatusMetric<T>
    {
        public static NullStatusMetric<T> Instance { get; private set; }

        private string metricId = "null_status_metric";

        static NullStatusMetric()
        {
            Instance = new NullStatusMetric<T>();
        }

        private NullStatusMetric()
        {
            Metadata = new Dictionary<string, string>();
        }

        public string MetricId
        {
            get { return metricId; }
        }

        public IDictionary<string, string> Metadata { get; private set; }

        public T Status
        {
            get { return default(T); }
        }
    }
}
