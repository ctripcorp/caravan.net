using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Metric;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Ribbon.Rule;
using System.Collections.Concurrent;
using Com.Ctrip.Soa.Caravan.Ribbon.Util;
using Com.Ctrip.Soa.Caravan.Collections.Generic;
using Com.Ctrip.Soa.Caravan.ValueCorrector;

namespace Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer
{
    class DefaultLoadBalancerContext : ILoadBalancerContext
    {
        private static ILog _logger = LogManager.GetLogger(typeof(DefaultLoadBalancerContext));

        private ConcurrentDictionary<string, ServerStats> serverStatsMap = new ConcurrentDictionary<string, ServerStats>();

        private string _refinedLoadBalancerId;

        private ILoadBalancerRuleFactoryManager _loadBalancerRuleFactoryManager;

        private IProperty<int?> _minAvailableServerCountProperty;
        private IProperty<int?> _globalMinAvailableServerCountProperty;
        
        internal LoadBalancerManager LoadBalancerManager { get; private set; }

        public string ManagerId { get; private set; }

        public string LoadBalancerId { get; private set; }

        public string LoadBalancerKey 
        { 
            get 
            {
                if (_refinedLoadBalancerId == null)
                    _refinedLoadBalancerId = string.Format("{0}.{1}", ManagerId, LoadBalancerId);
                return _refinedLoadBalancerId;
            } 
        }

        public IServerSource ServerSource { get; private set; }

        public ILoadBalancer LoadBalancer { get; private set; }

        public IServerSourceFilter ServerSourceFilter { get; private set; }

        public IServerSourceManager ServerSourceManager { get; private set; }

        public IServerSourceMonitor ServerSourceMonitor { get; private set; }

        public IConfigurationManager ConfigurationManager { get; private set; }

        public Dictionary<string, string> AdditionalInfo { get; private set; }

        public ILoadBalancerRule GetLoadBalancerRule(string ruleId)
        {
            return _loadBalancerRuleFactoryManager.GetLoadBalancerRule(ruleId);
        }

        public IPing Ping { get; internal set; }

        public IEventMetricManager EventMetricManager { get; private set; }

        public IAuditMetricManager AuditMetricManager { get; private set; }

        public IStatusMetricManager<double> StatusMetricManager { get; private set; }

        public int MinAvailableServerCount 
        {
            get
            {
                return _minAvailableServerCountProperty.Value ?? _globalMinAvailableServerCountProperty.Value.Value;
            }
        }

        public DefaultLoadBalancerContext(LoadBalancerManager loadBalancerManager, string loadBalancerId, LoadBalancerConfig loadBalancerConfig)
        {
            LoadBalancerManager = loadBalancerManager;
            ConfigurationManager = loadBalancerManager.LoadBalancerManagerConfig.ConfigurationManager;
            EventMetricManager = loadBalancerManager.LoadBalancerManagerConfig.EventMetricManager;
            AuditMetricManager = loadBalancerManager.LoadBalancerManagerConfig.AuditMetricManager;
            StatusMetricManager = loadBalancerManager.LoadBalancerManagerConfig.StatusMetricManager;

            ManagerId = loadBalancerManager.ManagerId;
            LoadBalancerId = loadBalancerId;
            Ping = loadBalancerConfig.Ping;
            ServerSource = loadBalancerConfig.ServerSource;
            AdditionalInfo = new Dictionary<string, string>() { { "LoadBalancerKey", LoadBalancerKey } };

            Initialize(loadBalancerConfig);
        }

        private void Initialize(LoadBalancerConfig loadBalancerConfig)
        {
            InitializeMinAvailableServerCountProperty();

            _loadBalancerRuleFactoryManager = new DefaultLoadBalancerRuleFactoryManager(this);
            if (loadBalancerConfig.RuleFactory != null)
                _loadBalancerRuleFactoryManager.RegisterLoadBalancerRuleFactory(loadBalancerConfig.RuleFactory);
            ServerSourceFilter = new DefaultServerSourceFilter(this);
            ServerSourceManager = new DefaultServerSourceManager(this);
            ServerSourceMonitor = new DefaultServerSourceMonitor(this);
            LoadBalancer = new DefaultLoadBalancer(this);

            ServerSourceFilter.OnChange += (o, e) => 
            {
                ServerSourceManager.Backup(ServerSourceFilter.LoadBalancerRoutes);
            };
            ServerSourceMonitor.ServerStatusChange += (o, e) => 
            {
                ServerSourceFilter.Refresh();
                ServerSourceManager.Backup(ServerSourceFilter.LoadBalancerRoutes);
            };

            if (ServerSource is IDynamicServerSource)
            {
                ((IDynamicServerSource)ServerSource).OnChange += (o, e) => 
                {
                    ServerSourceFilter.LoadBalancerRoutes = ServerSource.LoadBalancerRoutes;
                };
            }

            if (loadBalancerConfig.ServerSourceRestoreEventHandler != null)
                ServerSourceManager.OnServerSourceRestore += loadBalancerConfig.ServerSourceRestoreEventHandler;

            if (loadBalancerConfig.SeekRouteEventHandler != null)
                ServerSourceFilter.OnSeekRoute += loadBalancerConfig.SeekRouteEventHandler;

            var routes = ServerSource.LoadBalancerRoutes;
            var servers = new List<Server>();
            if (routes != null)
                routes.ForEach(route => servers.AddRange(route.Servers));
            if (servers.Count == 0)
            {
                routes = ServerSourceManager.Restore();
                routes.ForEach(route => servers.AddRange(route.Servers));
                var localCache = string.Join("\n", servers);
                _logger.Warn("Server list is null! Load server list from local cache." + localCache, AdditionalInfo);
            }
            if (servers.Count != 0)
                ServerSourceManager.Backup(ServerSourceFilter.LoadBalancerRoutes);
            ServerSourceFilter.LoadBalancerRoutes = routes;

            ServerSourceMonitor.MonitorServers();
        }

        private void InitializeMinAvailableServerCountProperty()
        {
            var globalMinAvailableServerCountPropertyKey = string.Format("{0}.{1}", ManagerId, ConfigurationKeys.MinAvailableServerCount);
            var minAvailableServerCountPropertyKey = string.Format("{0}.{1}", _refinedLoadBalancerId, ConfigurationKeys.MinAvailableServerCount);

            var valueCorrector = new NullableLowerBoundCorrector<int>(1);
            _globalMinAvailableServerCountProperty = ConfigurationManager.GetProperty<int?>(globalMinAvailableServerCountPropertyKey, 2, valueCorrector);
            _minAvailableServerCountProperty = ConfigurationManager.GetProperty<int?>(minAvailableServerCountPropertyKey, null, valueCorrector);
        }

        public ServerStats GetServerStats(Server server)
        {
            if (server == null)
                return null;
            return serverStatsMap.GetOrAdd(server.ServerId, key => new ServerStats(this));
        }
    }
}
