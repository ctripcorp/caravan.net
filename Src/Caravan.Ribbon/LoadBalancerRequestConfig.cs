using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Collections.Generic;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    public class LoadBalancerRequestConfig
    {
        private SortedSet<LoadBalancerRouteConfig> routeChains;

        public LoadBalancerRouteConfig[] RouteChains { get; private set; }

        public LoadBalancerRequestConfig(IEnumerable<LoadBalancerRouteConfig> requestConfig)
        {
            routeChains = new SortedSet<LoadBalancerRouteConfig>();
            if (requestConfig != null)
                requestConfig.ForEach(routeConfig => routeChains.Add(routeConfig));

            RouteChains = routeChains.ToArray();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{\"RouteChains\":[}");
            for (int i = 0; i < RouteChains.Length; i++)
            {
                var routeConfig = RouteChains[i];
                builder.Append(routeConfig);
                if (i + 1 != RouteChains.Length)
                    builder.Append(',');
            }
            builder.Append("]}");
            return builder.ToString();
        }
    }
}
