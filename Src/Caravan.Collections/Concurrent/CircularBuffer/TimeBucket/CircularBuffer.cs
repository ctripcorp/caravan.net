using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace Com.Ctrip.Soa.Caravan.Collections.Concurrent.CircularBuffer.TimeBucket.Buffer
{
    public abstract class CircularBuffer<T>
        where T : Bucket
    {
        protected const int LegacyBucketCount = 1;
        protected readonly T[] Buckets;
        protected int _bufferEnd;
        protected readonly object AddBucketLock = new object();
        protected readonly long TimeWindowInMilliseconds;
        protected readonly long BucketTimeWindowInMilliseconds;
        protected readonly int BucketCount;

        protected CircularBuffer(long timeWindowInMilliseconds, int bucketCount)
        {
            if (timeWindowInMilliseconds < 1)
                throw new ArgumentException("Time window cannot be less than 1.");
            if (bucketCount < 1)
                throw new ArgumentException("Bucket count cannot be less than 1.");
            if (timeWindowInMilliseconds % bucketCount != 0)
                throw new ArgumentException("Time window must be n * bucket time window.");

            TimeWindowInMilliseconds = timeWindowInMilliseconds;
            BucketCount = bucketCount;
            BucketTimeWindowInMilliseconds = timeWindowInMilliseconds / bucketCount;

            Buckets = new T[BucketCount + LegacyBucketCount];
            for (int i = 0; i < Buckets.Length; i++)
                Buckets[i] = CreateEmptyBucket(0);
        }

        protected abstract T CreateEmptyBucket(long timeInMilliseconds);

        protected long GetCurrentBucketStartTimeInMilliseconds()
        {
            long currentTimeInMilliseconds = CurrentTimeInMilliseconds;
            return currentTimeInMilliseconds - currentTimeInMilliseconds % BucketTimeWindowInMilliseconds;
        }

        protected T CurrentBucket
        {
            get
            {
                long currentBucketStartTime = GetCurrentBucketStartTimeInMilliseconds();
                if (Buckets[_bufferEnd].TimeInMilliseconds + BucketTimeWindowInMilliseconds <= currentBucketStartTime)
                {
                    if (Monitor.TryEnter(AddBucketLock))
                    {
                        try
                        {
                            if (Buckets[_bufferEnd].TimeInMilliseconds + BucketTimeWindowInMilliseconds <= currentBucketStartTime)
                            {
                                int newBufferEnd = (_bufferEnd + 1) % Buckets.Length;
                                Buckets[newBufferEnd] = CreateEmptyBucket(currentBucketStartTime);
                                _bufferEnd = newBufferEnd;
                            }
                        }
                        finally
                        {
                            Monitor.Exit(AddBucketLock);
                        }
                    }
                }

                return Buckets[_bufferEnd];
            }
        }

        private static long CurrentTimeInMilliseconds
        {
            get
            {
                return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }
        }
    }
}
