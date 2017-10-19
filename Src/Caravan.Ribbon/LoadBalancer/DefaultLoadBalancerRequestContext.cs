using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using Com.Ctrip.Soa.Caravan.Logging;

namespace Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer
{
    class DefaultLoadBalancerRequestContext : ILoadBalancerRequestContext
    {
        private static ILog _logger = LogManager.GetLogger(typeof(DefaultLoadBalancerRequestContext));

        public Server Server { get; private set; }

        internal ServerStats ServerStats { get; private set; }

        internal ILoadBalancerContext LoadBalancerContext { get; private set; }

        public DefaultLoadBalancerRequestContext(Server server, ILoadBalancerContext loadBalancerContext)
        {
            this.Server = server;
            this.LoadBalancerContext = loadBalancerContext;
            this.ServerStats = loadBalancerContext.GetServerStats(server);
        }

        public void MarkServerAvailable()
        {
            if (ServerStats == null)
                return;

            ServerStats.AddAvailableCount();
        }

        public void MarkServerUnavailable()
        {
            if (ServerStats == null)
                return;

            ServerStats.AddUnavailableCount();
        }
    }
}
