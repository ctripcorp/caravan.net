using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Threading.Atomic;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Algorithm
{
    public interface IRoundRobinAlgorithm<T>
    {
        T Choose(T[] source);
        T Choose(List<T> source);
    }

    public class DefaultRoundRobinAlgorithm<T> : IRoundRobinAlgorithm<T>
    {
        private static readonly int MaxValue = int.MaxValue / 2;

        private AtomicInteger counter = new AtomicInteger(new Random().Next(MaxValue));

        public T Choose(T[] items)
        {
            if (items == null || items.Length == 0)
                return default(T);

            int counterValue = counter.GetAndIncrement();
            if (counterValue > MaxValue)
                counter.GetAndSet(0);

            int index = counterValue % items.Length;
            return items[index];
        }

        public T Choose(List<T> items)
        {
            if (items == null || items.Count == 0)
                return default(T);

            int counterValue = counter.GetAndIncrement();
            if (counterValue > MaxValue)
                counter.GetAndSet(0);

            int index = counterValue % items.Count;
            return items[index];
        }
    }
}
