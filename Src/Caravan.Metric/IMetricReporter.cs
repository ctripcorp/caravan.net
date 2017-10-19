using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public interface IMetricReporter<T> where T : IMetric
    {
        void Report(T metric);
    }
}
