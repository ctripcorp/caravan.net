using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    internal class ConfigurationSourceComparer: IComparer<IConfigurationSource>
    {
        public int Compare(IConfigurationSource o1, IConfigurationSource o2)
        {
            if (o1 == o2)
                return 0;
            if (o1 == null)
                return 1;
            if (o2 == null)
                return -1;
            if (o1.Priority == o2.Priority)
                return 0;
            return o1.Priority < o2.Priority ? 1 : -1;
        }
    }
}
