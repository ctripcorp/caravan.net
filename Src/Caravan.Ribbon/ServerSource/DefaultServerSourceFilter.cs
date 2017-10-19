using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Metric;
using Com.Ctrip.Soa.Caravan.Ribbon.Util;
using Com.Ctrip.Soa.Caravan.Collections.Generic;
using System.Collections.Concurrent;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    class DefaultServerSourceFilter : IServerSourceFilter
    {
        private const string ServerCountMetricSuffix = "ribbon.servers.count";
        private const string ServerAvailableCountMetricSuffix = "ribbon.servers.available.count";
        
        private static ILog _logger = LogManager.GetLogger(typeof(DefaultServerSourceFilter));
        private ILoadBalancerContext _loadBalancerContext;

        private volatile bool _initialized;
        private volatile List<LoadBalancerRoute> _routes = new List<LoadBalancerRoute>();
        private volatile Dictionary<string, Server> _serverMap = new Dictionary<string, Server>();
        private volatile Dictionary<string, LoadBalancerRoute> _routeMap = new Dictionary<string, LoadBalancerRoute>();

        private IStatusMetricManager<double> _statusMetricManager;
        private readonly ConcurrentDictionary<String, IStatusMetric<double>> _serverCountMetrics;
        private readonly ConcurrentDictionary<String, IStatusMetric<double>> _availableServerCountMetrics;

        public DefaultServerSourceFilter(ILoadBalancerContext loadBalancerContext)
        {
            _loadBalancerContext = loadBalancerContext;

            _statusMetricManager = loadBalancerContext.StatusMetricManager;
            _serverCountMetrics = new ConcurrentDictionary<string, IStatusMetric<double>>();
            _availableServerCountMetrics = new ConcurrentDictionary<string, IStatusMetric<double>>();
        }

        public List<LoadBalancerRoute> LoadBalancerRoutes
        {
            get
            {
                return _routes;
            }
            set
            {
                if (value.IsNullOrEmpty()) 
                {
                    _logger.Info("Ignored null or empty routes", _loadBalancerContext.AdditionalInfo);
                    return;
                }

                value = value.FilterInvalidEntities(_logger, _loadBalancerContext.AdditionalInfo);

                var newServerMap = new Dictionary<string, Server>();
                PingUtil pingUtil = new PingUtil(_loadBalancerContext, _logger);
                foreach (LoadBalancerRoute newRoute in value)
                {
                    bool needPing = _initialized && newRoute.Servers.Length > _loadBalancerContext.MinAvailableServerCount;
                    foreach (var newServer in newRoute.Servers)
                    {
                        Server server;
                        if (_serverMap.TryGetValue(newServer.ServerId, out server))
                            newServer.IsAlive = server.IsAlive;
                        else if (needPing && pingUtil.HasPing)
                            newServer.IsAlive = pingUtil.IsAlive(newServer);
                        newServerMap[newServer.ServerId] = newServer;
                    }
                }

                var newRouteMap = BuildRouteMaps(value);
                if (newRouteMap.IsNullOrEmpty())
                    return;

                var message = string.Format("Routes changed from\n{0}\nto\n{1}", string.Join("\n", _routes), string.Join("\n", value));
                _logger.Info(message, _loadBalancerContext.AdditionalInfo);
                
                InitMetric(value);

                _routes = value;
                _routeMap = newRouteMap;
                _serverMap = newServerMap;
                _initialized = true;

                Refresh();
                RaiseServerContextChangeEvent();
            }
        }

        private Dictionary<string, LoadBalancerRoute> BuildRouteMaps(List<LoadBalancerRoute> newRoutes) 
        {
            var newRouteMap = new Dictionary<string, LoadBalancerRoute>();
            foreach (LoadBalancerRoute newRoute in newRoutes)
            {
                if (newRoute.Servers.Length == 0)
                {
                    _logger.Info("Ignore empty route:" + newRoute.RouteId, _loadBalancerContext.AdditionalInfo);
                    continue;
                }

                newRouteMap[newRoute.RouteId] = newRoute;
                if (!_routeMap.ContainsKey(newRoute.RouteId))
                    _logger.Info("Add new route: " + newRoute.RouteId, _loadBalancerContext.AdditionalInfo);
                else
                    _logger.Info("Update route: " + newRoute.RouteId, _loadBalancerContext.AdditionalInfo);
            }
            return newRouteMap;
        }

        private void PingServers(LoadBalancerRoute route)
        {
            PingUtil pingUtil = new PingUtil(_loadBalancerContext, _logger);
            if (pingUtil.HasPing)
            {
                foreach (ServerGroup serverGroup in route.ServerGroups)
                {
                    foreach (Server server in serverGroup.Servers)
                    {
                        server.IsAlive = pingUtil.IsAlive(server);
                    }
                }
            }
        }

        public LoadBalancerRoute GetLoadBalancerRoute(LoadBalancerRequestConfig requestConfig)
        {
            if (requestConfig == null || requestConfig.RouteChains.IsNullOrEmpty())
                return null;

            var routeMap = _routeMap;
            for (int i = 0; i < requestConfig.RouteChains.Length; i++)
            {
                LoadBalancerRouteConfig routeConfig = requestConfig.RouteChains[i];
                if (!string.IsNullOrWhiteSpace(routeConfig.RouteId))
                {
                    LoadBalancerRoute route;
                    if (routeMap.TryGetValue(routeConfig.RouteId, out route))
                    {
                        var e = new SeekRouteEventArgs(route);
                        RaiseSeekRouteEvent(e);
                        return e.Route;
                    }
                }
                if (!routeConfig.AllowFallback)
                    return null;
            }
            return null;
        }

        public void Refresh()
        {
            lock (this)
            {
                List<LoadBalancerRoute> routes = _routes;
                foreach (LoadBalancerRoute route in routes)
                {
                    foreach (ServerGroup serverGroup in route.ServerGroups)
                    {
                        serverGroup.RefreshAvailableServers();
                    }
                }
            }
        }

        public event EventHandler OnChange;

        public event EventHandler<SeekRouteEventArgs> OnSeekRoute;

        private void RaiseServerContextChangeEvent()
        {
            if (OnChange == null)
                return;

            try
            {
                OnChange(this, new EventArgs());
            }
            catch (Exception ex)
            {
                _logger.Warn("Error occurred while raising ServerContextChangeEvent.", ex, _loadBalancerContext.AdditionalInfo);
            }
        }

        private void RaiseSeekRouteEvent(SeekRouteEventArgs e)
        {
            if (OnSeekRoute == null)
                return;

            try
            {
                OnSeekRoute(this, e);
            }
            catch (Exception ex)
            {
                _logger.Warn("Error occurred while raising SeekRouteEvent.", ex, _loadBalancerContext.AdditionalInfo);
            }
        }

        private void InitMetric(List<LoadBalancerRoute> routes)
        {
            foreach (var route in routes)
            {
                if (string.IsNullOrWhiteSpace(route.RouteId))
                    continue;

                InitServerCountMetric(route);
                InitAvailableServerCountMetric(route);
            }
        }

        private void InitServerCountMetric(LoadBalancerRoute route)
        {
            var metricId = _loadBalancerContext.GetMetricId(route.RouteId + "." + ServerCountMetricSuffix);
            var metricName = _loadBalancerContext.GetMetricName(ServerCountMetricSuffix);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata["metric_name_audit"] = metricName;
            metadata["loadbalancerid"] = _loadBalancerContext.LoadBalancerId;
            metadata["routeid"] = route.RouteId;
            var metricConfig = new StatusMetricConfig<double>(() => route.Servers.Length, metadata);
            _serverCountMetrics.GetOrAdd(route.RouteId, key => _statusMetricManager.GetMetric(metricId, metricConfig));
        }

        private void InitAvailableServerCountMetric(LoadBalancerRoute route)
        {
            var metricId = _loadBalancerContext.GetMetricId(route.RouteId + "." + ServerAvailableCountMetricSuffix);
            var metricName = _loadBalancerContext.GetMetricName(ServerAvailableCountMetricSuffix);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata["metric_name_audit"] = metricName;
            metadata["loadbalancerid"] = _loadBalancerContext.LoadBalancerId;
            metadata["routeid"] = route.RouteId;
            var metricConfig = new StatusMetricConfig<double>(() => route.AvailableServers.Length, metadata);
            _availableServerCountMetrics.GetOrAdd(route.RouteId, key => _statusMetricManager.GetMetric(metricId, metricConfig));
        }
    }
}
