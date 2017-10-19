using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Ribbon.Rule;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    public class LoadBalancerManager
    {
        private static ConcurrentDictionary<string, LoadBalancerManager> _managerCache = new ConcurrentDictionary<string, LoadBalancerManager>();

        public string ManagerId { get; private set; }
        
        public LoadBalancerManagerConfig LoadBalancerManagerConfig { get; private set; }

        private ConcurrentDictionary<string, ILoadBalancer> _loadBalancerCache = new ConcurrentDictionary<string, ILoadBalancer>();

        private LoadBalancerManager(string managerId, LoadBalancerManagerConfig loadBalancerManagerConfig)
        {
            ParameterChecker.NotNullOrWhiteSpace(managerId, "managerId");
            ParameterChecker.NotNull(loadBalancerManagerConfig, "loadBalancerManagerConfig");

            ManagerId = managerId;
            LoadBalancerManagerConfig = loadBalancerManagerConfig;
        }

        private ILoadBalancer NewLoadBalancer(string loadBalancerId, LoadBalancerConfig loadBalancerConfig)
        {
            ParameterChecker.NotNullOrWhiteSpace(loadBalancerId, "loadBalancerId");
            ParameterChecker.NotNull(loadBalancerConfig, "loadBalancerConfig");
            var lbContext = new DefaultLoadBalancerContext(this, loadBalancerId, loadBalancerConfig);
            return lbContext.LoadBalancer;
        }

        public ILoadBalancer GetLoadBalancer(string loadBalancerId, LoadBalancerConfig loadBalancerConfig)
        {
            return _loadBalancerCache.GetOrAdd(loadBalancerId, key => NewLoadBalancer(key, loadBalancerConfig));
        }

        public static LoadBalancerManager GetManager(string factoryId, LoadBalancerManagerConfig loadBalancerManagerConfig)
        {
            return _managerCache.GetOrAdd(factoryId, key => new LoadBalancerManager(key, loadBalancerManagerConfig));
        }
    }
}
