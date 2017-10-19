using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Collections.Generic;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    class DefaultDynamicServerSource : DefaultServerSource, IDynamicServerSource
    {
        public event EventHandler OnChange;

        public override List<LoadBalancerRoute> LoadBalancerRoutes
        {
            get
            {
                return base.LoadBalancerRoutes;
            }
            set
            {
                base.LoadBalancerRoutes = value;
                if (OnChange != null)
                    OnChange(this, new EventArgs());
            }
        }

        public DefaultDynamicServerSource(IEnumerable<LoadBalancerRoute> routes)
            : base(routes)
        {
        }

        public DefaultDynamicServerSource(IEnumerable<Server> servers)
            : base(servers)
        {
        }

        public DefaultDynamicServerSource(string routeId, IEnumerable<Server> servers)
            : base(routeId, servers)
        {
        }

        public DefaultDynamicServerSource(IEnumerable<ServerGroup> serverGroups)
            : base(serverGroups)
        {
        }

        public DefaultDynamicServerSource(string routeId, IEnumerable<ServerGroup> serverGroups)
            : base(routeId, serverGroups)
        {
        }
    }
}
