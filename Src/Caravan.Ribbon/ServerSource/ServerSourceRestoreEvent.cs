using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    public class ServerSourceRestoreEvent : EventArgs
    {
        public List<LoadBalancerRoute> Routes { get; set; }

        internal ServerSourceRestoreEvent(List<LoadBalancerRoute> routes)
        {
            Routes = routes;
        }
    }
}
