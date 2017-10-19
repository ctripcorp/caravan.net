using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public interface IAuditMetricManager : IMetricManager<IAuditMetric>
    {
        IAuditMetric GetMetric(string metricId, MetricConfig metricConfig);
    }
}
