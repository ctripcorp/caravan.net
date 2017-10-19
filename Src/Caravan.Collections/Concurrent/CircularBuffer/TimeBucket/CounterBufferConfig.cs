using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Collections.Concurrent.CircularBuffer.TimeBucket
{
    public class CounterBufferConfig
    {
        public long BufferTimeWindow { get; private set; }

        public long BucketTimeWindow { get; private set; }

        public int BucketCount { get; private set; }

        public CounterBufferConfig(long bufferTimeWindow, long bucketTimeWindow)
        {
            if (bufferTimeWindow <= 0)
                throw new ArgumentException("Buffer time window cannot be <= 0.");

            if (bucketTimeWindow <= 0)
                throw new ArgumentException("Bucket time window cannot be <= 0.");

            long bucketCount = bufferTimeWindow / bucketTimeWindow;
            if (bucketCount * bucketTimeWindow != bufferTimeWindow)
            {
                string message = string.Format("Buffer time window {0} cannot be divided by bucket time window {1}.", bufferTimeWindow, bucketTimeWindow);
                throw new ArgumentException(message);
            }

            if (bucketCount > int.MaxValue)
            {
                string message = string.Format("bucket count {0} cannot be > Integer.MAX_VALUE", bucketCount);
                throw new ArgumentException(message);
            }

            BufferTimeWindow = bufferTimeWindow;
            BucketTimeWindow = bucketTimeWindow;
            BucketCount = (int)bucketCount;
        }
    }
}
