using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric.Null
{
    public class NullEventMetricReporter : IMetricReporter<IEventMetric>
    {
        public static NullEventMetricReporter Instance { get; private set; }

        static NullEventMetricReporter()
        {
            Instance = new NullEventMetricReporter();
        }

        private NullEventMetricReporter()
        { }

        public void Report(IEventMetric metric)
        { }
    }
}
