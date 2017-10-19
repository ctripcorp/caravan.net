using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Threading.Atomic;
using Com.Ctrip.Soa.Caravan.Collections.Generic;
using Com.Ctrip.Soa.Caravan.Ribbon.Algorithm;
using Com.Ctrip.Soa.Caravan.Ribbon.Util;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Rule
{
    public class RoundRobinRule : ILoadBalancerRule
    {
        internal volatile LoadBalancerRoute loadBalancerRoute;
        internal volatile List<ServerGroup> serverGroups;
        internal IRoundRobinAlgorithm<ServerGroup> roundRobinAlgorithm;
        internal volatile ConcurrentDictionary<string, RoundRobinContext> serverContexts;

        public virtual string Id
        {
            get { return DefaultLoadBalancerRuleFactoryManager.RoundRobinRuleName; }
        }

        public virtual string Description
        {
            get { return DefaultLoadBalancerRuleFactoryManager.RoundRobinRuleDescription; }
        }

        public RoundRobinRule()
        {
            roundRobinAlgorithm = new DefaultRoundRobinAlgorithm<ServerGroup>();
            serverContexts = new ConcurrentDictionary<string, RoundRobinContext>();
        }

        public virtual Server Choose(LoadBalancerRoute route)
        {
            if (route == null || route.ServerGroups.IsNullOrEmpty())
                return null;

            InternalBuildLoadBalanceItems(route);
            ServerGroup serverGroup = null;
            for (int i = 0; i < serverGroups.Count; i++)
            {
                serverGroup = roundRobinAlgorithm.Choose(serverGroups);
                if (serverGroup.AvailableServers.Length > 0)
                    break;
            }
            if (serverGroup == null)
                return null;

            var serverContext = serverContexts.GetOrAdd(serverGroup.GroupId, key => new RoundRobinContext(serverGroup));
            return serverContext.Choose();
        }

        protected void InternalBuildLoadBalanceItems(LoadBalancerRoute route)
        {
            if (!Object.ReferenceEquals(loadBalancerRoute, route))
            {
                lock (this)
                {
                    if (!Object.ReferenceEquals(loadBalancerRoute, route))
                    {
                        serverGroups = BuildLoadBalanceItems(route);
                        serverContexts = new ConcurrentDictionary<string, RoundRobinContext>();
                        loadBalancerRoute = route;
                    }
                }
            }
        }

        protected virtual List<ServerGroup> BuildLoadBalanceItems(LoadBalancerRoute route)
        {
            return new List<ServerGroup>(route.ServerGroups);
        }
    }
}
