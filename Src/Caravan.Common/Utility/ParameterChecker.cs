using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Utility
{
    public static class ParameterChecker
    {
        public static void NotNull(object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        public static void NotNullOrWhiteSpace(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(string.Format("Argument \"{0}\" is null or white space", name));
        }

        public static void NotNullOrEmpty<T>(ICollection<T> collection, string name)
        {
            if (collection == null || collection.Count == 0)
                throw new ArgumentException(string.Format("Argument \"{0}\" is null or empty", name));
        }
    }
}
