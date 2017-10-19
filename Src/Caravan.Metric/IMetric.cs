using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public interface IMetric
    {
        string MetricId { get; }

        IDictionary<string, string> Metadata { get; }
    }
}
