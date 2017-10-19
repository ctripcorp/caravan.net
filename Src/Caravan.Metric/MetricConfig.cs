using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public class MetricConfig
    {
        public IDictionary<string, string> Metadata { get; private set; }

        public MetricConfig(IDictionary<string, string> metadata)
        {
            Metadata = metadata == null ? new Dictionary<string, string>() : new Dictionary<string, string>(metadata);
        }
    }
}
