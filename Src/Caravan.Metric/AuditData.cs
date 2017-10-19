using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Metric
{
    public class AuditData
    {
        public virtual long Count { get; set; }

        public virtual long Sum { get; set; }

        public virtual long Min { get; set; }

        public virtual long Max { get; set; }

        public long Average 
        {
            get
            {
                long count = this.Count;
                if(count == 0)
                    return 0;
                return Sum / count;
            }
        }

        public AuditData()
        { }

        public AuditData(long count, long sum, long min, long max)
        {
            Count = count;
            Sum = sum;
            Min = min;
            Max = max;
        }
    }
}
