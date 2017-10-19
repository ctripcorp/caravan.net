using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    public class SeekRouteEventArgs : EventArgs
    {
        public LoadBalancerRoute Route { get; set; }

        internal SeekRouteEventArgs(LoadBalancerRoute route)
        {
            Route = route;
        }
    }
}
