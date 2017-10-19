using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric.Null
{
    public class NullStatusMetricReporter<T> : IMetricReporter<IStatusMetric<T>>
    {
        public static NullStatusMetricReporter<T> Instance { get; private set; }

        static NullStatusMetricReporter()
        {
            Instance = new NullStatusMetricReporter<T>();
        }

        private NullStatusMetricReporter()
        { }

        public void Report(IStatusMetric<T> metric)
        { }
    }
}
