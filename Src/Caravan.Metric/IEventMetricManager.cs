using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public interface IEventMetricManager : IMetricManager<IEventMetric>
    {
        IEventMetric GetMetric(string metricId, MetricConfig metricConfig);
    }
}
