using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public interface IMetricManager<T> where T : IMetric
    {
        string ManagerId { get; }

        MetricManagerConfig<T> Config { get; }

        IEnumerable<T> Metrics { get; }
    }
}
