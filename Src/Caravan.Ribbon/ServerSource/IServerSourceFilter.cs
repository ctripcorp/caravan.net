using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    interface IServerSourceFilter
    {
        List<LoadBalancerRoute> LoadBalancerRoutes { get; set; }

        LoadBalancerRoute GetLoadBalancerRoute(LoadBalancerRequestConfig requestConfig);

        void Refresh();

        event EventHandler OnChange;

        event EventHandler<SeekRouteEventArgs> OnSeekRoute;
    }
}
