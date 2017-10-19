using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public interface IAuditMetric : IMetric
    {
        AuditData AuditData { get; }
        
        void AddValue(double value);

        long GetCount();

        long GetCountInRange(long lowerBound, long upperBound);

        long GetPercentile(double percent);
    }
}
