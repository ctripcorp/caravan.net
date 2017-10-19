using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric.Null
{
    public class NullAuditMetric : IAuditMetric
    {
        public static NullAuditMetric Instance { get; private set; }

        private string metricId = "null_audit_metric";

        static NullAuditMetric()
        {
            Instance = new NullAuditMetric();
        }

        private NullAuditMetric()
        {
            Metadata = new Dictionary<string, string>();
            AuditData = new NullMetricAuditData();
        }

        public string MetricId
        {
            get { return metricId; }
        }

        public IDictionary<string, string> Metadata { get; private set; }

        public AuditData AuditData { get; private set; }

        public void AddValue(double value)
        { }

        public long GetCount()
        {
            return 0;
        }

        public long GetCountInRange(long lowerBound, long upperBound)
        {
            return 0;
        }

        public long GetPercentile(double percent)
        {
            return 0;
        }

        private class NullMetricAuditData : AuditData
        {
            private long max = 0, min = 0, sum = 0, count = 0;

            public override long Count 
            {
                get { return count; }
                set { throw new InvalidOperationException("AuditData of NullAuditMetric cannot be modified."); }
            }

            public override long Min
            {
                get { return min; }
                set { throw new InvalidOperationException("AuditData of NullAuditMetric cannot be modified."); }
            }

            public override long Max
            {
                get { return max; }
                set { throw new InvalidOperationException("AuditData of NullAuditMetric cannot be modified."); }
            }

            public override long Sum
            {
                get { return sum; }
                set { throw new InvalidOperationException("AuditData of NullAuditMetric cannot be modified."); }
            }
        }
    }
}
