using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Com.Ctrip.Soa.Caravan.Extensions
{
    /// <summary>
    /// IEnumerableExtension 类
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// 对集合中的所有元素执行 action 操作
        /// </summary>
        /// <param name="items">集合</param>
        /// <param name="action">要执行的操作</param>
        public static void Foreach(this IEnumerable items, Action<object> action)
        {
            foreach (var item in items)
                action(item);
        }

        /// <summary>
        /// 对集合中的所有元素执行 action 操作
        /// </summary>
        /// <param name="items">集合</param>
        /// <param name="action">要执行的操作</param>
        public static void Foreach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
                action(item);
        }
    }
}
