using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric.Null
{
    public class NullEventMetric : IEventMetric
    {
        public static NullEventMetric Instance { get; private set; }

        private string metricId = "null_event_metric";

        static NullEventMetric()
        {
            Instance = new NullEventMetric();
        }

        private NullEventMetric()
        {
            Metadata = new Dictionary<string, string>();
            EventTypes = new List<string>();
        }

        public string MetricId
        {
            get { return metricId; }
        }

        public IDictionary<string, string> Metadata { get; private set; }

        public IEnumerable<string> EventTypes { get; private set; }

        public void AddEvent(string eventType)
        { }

        public long GetCount()
        {
            return 0;
        }

        public long GetCount(string eventType)
        {
            return 0;
        }
    }
}
