using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public interface IEventMetric : IMetric
    {
        IEnumerable<string> EventTypes { get; }

        void AddEvent(string eventType);

        long GetCount();

        long GetCount(string eventType);
    }
}
