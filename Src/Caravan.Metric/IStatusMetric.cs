using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public interface IStatusMetric<T> : IMetric
    {
        T Status { get; }
    }
}
