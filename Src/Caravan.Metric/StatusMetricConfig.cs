using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public class StatusMetricConfig<T> : MetricConfig
    {
        public Func<T> StatusFactory { get; private set; }

        public StatusMetricConfig(Func<T> statusFactory, IDictionary<string, string> metadata)
            : base(metadata)
        {
            ParameterChecker.NotNull(statusFactory, "statusFactory");
            StatusFactory = statusFactory;
        }
    }
}
