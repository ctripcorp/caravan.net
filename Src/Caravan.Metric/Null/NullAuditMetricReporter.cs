using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric.Null
{
    public class NullAuditMetricReporter : IMetricReporter<IAuditMetric>
    {
        public static NullAuditMetricReporter Instance { get; private set; }

        static NullAuditMetricReporter()
        {
            Instance = new NullAuditMetricReporter();
        }

        private NullAuditMetricReporter()
        { }

        public void Report(IAuditMetric metric)
        { }
    }
}
