using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Collections.Concurrent.CircularBuffer.TimeBucket
{
    public class PercentileBufferConfig : CounterBufferConfig
    {
        public int BucketCapacity { get; private set; }

        public PercentileBufferConfig(long bufferTimeWindow, long bucketTimeWindow, int bucketCapacity)
            : base(bufferTimeWindow, bucketTimeWindow)
        {
            if (bucketCapacity <= 0)
                throw new ArgumentException("Bucket capacity cannot be <= 0.");

            BucketCapacity = bucketCapacity;
        }
    }
}
