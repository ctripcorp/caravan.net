using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public interface IStatusMetricManager<T> : IMetricManager<IStatusMetric<T>>
    {
        IStatusMetric<T> GetMetric(string metricId, StatusMetricConfig<T> metricConfig);
    }
}
