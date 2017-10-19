using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Com.Ctrip.Soa.Caravan.Collections.Generic
{
    public static class IEnumerableExtension
    {
        public static void ForEach(this IEnumerable source, Action<object> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
