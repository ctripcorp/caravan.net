using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public class MetricManagerConfig<T> where T : IMetric
    {
        public IMetricReporter<T> MetricReporter { get; private set; }

        public MetricManagerConfig(IMetricReporter<T> metricReporter)
        {
            ParameterChecker.NotNull(metricReporter, "metricReporter");
            MetricReporter = metricReporter;
        }
    }
}
