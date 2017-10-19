using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Com.Ctrip.Soa.Caravan.Collections.Generic
{
    public static class ListExtensions
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            if (list.IsNullOrEmpty())
                return;

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }
    }
}
