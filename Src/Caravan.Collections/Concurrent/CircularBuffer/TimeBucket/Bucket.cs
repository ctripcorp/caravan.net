using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Collections.Concurrent.CircularBuffer.TimeBucket.Buffer
{
    public abstract class Bucket
    {
        public long TimeInMilliseconds { get; protected set; }

        protected Bucket(long timeInMilliseconds)
        {
            TimeInMilliseconds = timeInMilliseconds;
        }
    }
}
