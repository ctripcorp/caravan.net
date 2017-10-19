using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    class DefaultServerSource : IServerSource
    {
        public virtual List<LoadBalancerRoute> LoadBalancerRoutes { get; set; }

        public DefaultServerSource(IEnumerable<LoadBalancerRoute> routes)
        {
            ParameterChecker.NotNull(routes, "routes");

            LoadBalancerRoutes = new List<LoadBalancerRoute>(routes);
        }

        public DefaultServerSource(IEnumerable<Server> servers)
            : this("default", servers)
        {
        }

        public DefaultServerSource(string routeId, IEnumerable<Server> servers)
        {
            ParameterChecker.NotNullOrWhiteSpace(routeId, "routeId");
            ParameterChecker.NotNull(servers, "servers");

            var serverGroups = new List<ServerGroup>() { new ServerGroup("default", 1, servers) };
            this.LoadBalancerRoutes = new List<LoadBalancerRoute>() { new LoadBalancerRoute(routeId, serverGroups) };
        }

        public DefaultServerSource(IEnumerable<ServerGroup> serverGroups)
            : this("default", serverGroups)
        {

        }

        public DefaultServerSource(string routeId, IEnumerable<ServerGroup> serverGroups)
        {
            ParameterChecker.NotNullOrWhiteSpace(routeId, "routeId");
            ParameterChecker.NotNull(serverGroups, "serverGroups");

            this.LoadBalancerRoutes = new List<LoadBalancerRoute>() { new LoadBalancerRoute(routeId, serverGroups) };
        }
    }
}
