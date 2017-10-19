using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using Com.Ctrip.Soa.Caravan.Logging;
using System.Collections.Concurrent;
using Com.Ctrip.Soa.Caravan.Ribbon.Rule;

namespace Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer
{
    class DefaultLoadBalancer : ILoadBalancer
    {
        private static ILog _logger = LogManager.GetLogger(typeof(DefaultLoadBalancer));

        private ConcurrentDictionary<string, ILoadBalancerRule> loadBalancerRules;
        internal ILoadBalancerContext LoadBalancerContext { get; private set; }

        public DefaultLoadBalancer(ILoadBalancerContext loadBalancerContext)
        {
            LoadBalancerContext = loadBalancerContext;
            loadBalancerRules = new ConcurrentDictionary<string, ILoadBalancerRule>();
        }

        public ILoadBalancerRequestContext GetRequestContext(LoadBalancerRequestConfig requestConfig)
        {
            LoadBalancerRoute route = LoadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig);
            if (route == null)
            {
                _logger.Warn("No matching route for \n" + requestConfig, LoadBalancerContext.AdditionalInfo);
                return null;
            }
            Server server = LoadBalancerContext.GetLoadBalancerRule(route.RouteId).Choose(route);
            return server == null ? null : new DefaultLoadBalancerRequestContext(server, LoadBalancerContext);
        }
    }
}
