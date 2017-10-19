using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Collections.Concurrent.CircularBuffer.TimeBucket.Buffer
{
    public class IntegerPercentileBuffer : PercentileBuffer<long>
    {
        public IntegerPercentileBuffer(long timeWindowInSeconds, int bucketTimeWindowInSeconds, int bucketSizeLimit)
            : base(timeWindowInSeconds, bucketTimeWindowInSeconds, bucketSizeLimit)
        {
        }

        public IntegerPercentileBuffer(PercentileBufferConfig percentileBufferConfig)
            : base(percentileBufferConfig)
        {
        }

        public void GetAuditData(out long count, out long sum, out long min, out long max)
        {
            long iCount = 0;
            long iSum = 0;
            long iMin = long.MaxValue;
            long iMax = long.MinValue;
            VisitData(
                item =>
                {
                    iCount++;
                    iSum += item;
                    if (item < iMin)
                        iMin = item;
                    if (item > iMax)
                        iMax = item;
                });
            if (iCount == 0)
            {
                iMin = 0;
                iMax = 0;
            }

            count = iCount;
            sum = iSum;
            min = iMin;
            max = iMax;
        }

        public int GetItemCountInRange(long low, long? high = null)
        {
            int count = 0;
            VisitData(
                item =>
                {
                    if (item >= low && (!high.HasValue || item < high.Value))
                        count++;
                });

            return count;
        }

        public long GetPercentile(double percent)
        {
            List<long> snapshot = GetSnapShot();

            if (snapshot.Count <= 0)
                return 0;

            snapshot.Sort();

            if (percent <= 0.0)
                return snapshot[0];

            if (percent >= 100.0)
                return snapshot[snapshot.Count - 1];

            int rank = (int)(percent * (snapshot.Count - 1) / 100);
            return snapshot[rank];
        }

        public long GetAuditDataAvg()
        {
            int count = 0;
            long sum = 0;
            VisitData(
                item =>
                {
                    count++;
                    sum += item;
                });

            long avg = 0;
            if (count > 0)
                avg = (long)Math.Round((double)sum / count);
            return avg;
        }
    }
}
