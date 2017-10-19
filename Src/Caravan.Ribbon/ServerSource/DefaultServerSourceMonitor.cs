using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Threading.Tasks;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using Com.Ctrip.Soa.Caravan.Ribbon.Extensions;
using Com.Ctrip.Soa.Caravan.Metric;
using Com.Ctrip.Soa.Caravan.Ribbon.Util;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    class DefaultServerSourceMonitor : IServerSourceMonitor
    {
        private static ILog _logger = LogManager.GetLogger(typeof(DefaultServerSourceMonitor));
        
        private const string CheckHealthLatencyMetricSuffix = "ribbon.checkhealth.latency";
        private const string CheckHealthEventMetricSuffix = "ribbon.checkhealth.event";
        
        private const string AvailabilityKey = "Availability";
        
        private const int DefaultFailureThresholdPercent = 50;
        private const int FailureThresholdPercentLowerBound = 0;
        private const int failureThresholdPercentUpperBound = 100;

        private const int DefaultCheckInterval = 2000;
        private const int CheckIntervalLowerBound = 1000;
        private const int CheckIntervalUpperBound = 10000;

        private Timer _timer;
        private volatile bool _isChecking;
        private volatile bool _lastHasUnavailableServers;
        private ConcurrentDictionary<string, IAuditMetric> _checkHealthLatencyMetrics;
        private ConcurrentDictionary<string, IEventMetric> _checkHealthEventMetrics;

        internal IProperty<int?> CheckIntervalInMillisecondProperty { get; private set; }
        internal IProperty<int?> FailureThresholdPercentageProperty { get; private set; }
        internal IProperty<int?> GlobalCheckIntervalInMillisecondProperty { get; private set; }
        internal IProperty<int?> GlobalFailureThresholdPercentageProperty { get; private set; }

        private ILoadBalancerContext _loadBalancerContext;
        private PingUtil _pingUtil;

        public DefaultServerSourceMonitor(ILoadBalancerContext loadBalancerContext)
        {
            _loadBalancerContext = loadBalancerContext;
            _pingUtil = new PingUtil(_loadBalancerContext, _logger);

            var configurationManager = _loadBalancerContext.ConfigurationManager;
            string prefix = _loadBalancerContext.LoadBalancerKey;
            CheckIntervalInMillisecondProperty = configurationManager.GetProperty<int?>(prefix + "." + ConfigurationKeys.CheckInterval);
            FailureThresholdPercentageProperty = configurationManager.GetProperty<int?>(prefix + "." + ConfigurationKeys.FailureThresholdPercentage);

            string globalPrefix = _loadBalancerContext.ManagerId;
            GlobalCheckIntervalInMillisecondProperty = configurationManager.GetProperty<int?>(globalPrefix + "." + ConfigurationKeys.CheckInterval, DefaultCheckInterval);
            GlobalFailureThresholdPercentageProperty = configurationManager.GetProperty<int?>(globalPrefix + "." + ConfigurationKeys.FailureThresholdPercentage, DefaultFailureThresholdPercent);

            _checkHealthLatencyMetrics = new ConcurrentDictionary<string, IAuditMetric>();
            _checkHealthEventMetrics = new ConcurrentDictionary<string, IEventMetric>();
        }

        public void MonitorServers()
        {
            if (_timer != null)
                return;

            lock (this)
            {
                if (_timer != null)
                    return;

                _timer = new Timer();
                OnIntervalChange(null, null);
                _timer.Elapsed += RunScheduledTask;
                _timer.Enabled = true;

                CheckIntervalInMillisecondProperty.OnChange += OnIntervalChange;
                GlobalCheckIntervalInMillisecondProperty.OnChange += OnIntervalChange;

                FailureThresholdPercentageProperty.OnChange += OnThresholdChange;
                GlobalFailureThresholdPercentageProperty.OnChange += OnThresholdChange;
            }
        }

        internal int CheckIntervalInMillisecond
        {
            get 
            {
                return CheckIntervalInMillisecondProperty.Value ?? GlobalCheckIntervalInMillisecondProperty.Value.Value;
            }
        }

        internal int FailureThresholdPercentage
        {
            get
            {
                return FailureThresholdPercentageProperty.Value ?? GlobalFailureThresholdPercentageProperty.Value.Value;
            }
        }

        private void OnIntervalChange(object sender, PropertyChangedEventArgs<int?> e)
        {
            int interval = (int)_timer.Interval;
            int newInterval = CheckIntervalInMillisecond;
            if (interval == newInterval)
                return;

            _timer.Interval = CheckIntervalInMillisecond;
        }

        private void OnThresholdChange(object sender, PropertyChangedEventArgs<int?> e)
        {
            FailureThresholdPercentageProperty.Refresh();
            GlobalFailureThresholdPercentageProperty.Refresh();
        }

        private void RunScheduledTask(object sender, ElapsedEventArgs e)
        {
            if (_isChecking)
                return;
            try
            {
                _isChecking = true;

                bool statusChanged = false;
                var routes = _loadBalancerContext.ServerSourceFilter.LoadBalancerRoutes;
                foreach (LoadBalancerRoute route in routes)
                {
                    statusChanged |= CheckHealth(route);
                }
                if (statusChanged)
                {
                    RaisingServerStatusChangeEvent();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Error occurred while run check health task", ex, _loadBalancerContext.AdditionalInfo);
            }
            finally
            {
                _isChecking = false;
            }
        }

        private bool CheckHealth(LoadBalancerRoute route, Server server)
        {
            if (!server.IsAlive)
            {
                if (!_pingUtil.HasPing)
                    return server.IsAlive;

                bool pingSuccess = _loadBalancerContext.Ping.IsAlive(server);
                if (pingSuccess)
                {
                    server.IsAlive = true;
                    string message = string.Format("Server({0}) is available due to ping success", server);
                    _logger.Info(message, _loadBalancerContext.AdditionalInfo.Copy().With(AvailabilityKey, bool.TrueString));
                    var checkHealthEventMetric = GetCheckHealthEventMetric(route, server.ServerId);
                    checkHealthEventMetric.AddEvent("PullIn");
                }
                return pingSuccess;
            }
            else
            {
                if (route.AvailableServers.Length <= _loadBalancerContext.MinAvailableServerCount)
                    return true;

                int successCount, failureCount, totalCount, failurePercent;
                bool failureRateExceeded, pingSuccess;
                var serverStats = _loadBalancerContext.GetServerStats(server);
                successCount = serverStats.GetAvailableCount();
                failureCount = serverStats.GetUnavailableCount();
                totalCount = successCount + failureCount;
                if (totalCount == 0)
                    return server.IsAlive;

                failurePercent = (int)(failureCount * 100.0 / totalCount);
                failureRateExceeded = failurePercent >= FailureThresholdPercentage;
                if (!failureRateExceeded)
                    return true;

                if (!_pingUtil.HasPing)
                {
                    server.IsAlive = false;
                    string message = string.Format("Server({0}) is unavailable due to high failure rate({1}%)", server, failurePercent);
                    _logger.Warn(message, _loadBalancerContext.AdditionalInfo.Copy().With(AvailabilityKey, bool.FalseString));
                    var checkHealthEventMetric = GetCheckHealthEventMetric(route, server.ServerId);
                    checkHealthEventMetric.AddEvent("PullOut");
                    return false;
                }

                pingSuccess = _pingUtil.IsAlive(server);
                if (!pingSuccess)
                {
                    server.IsAlive = false;
                    string message = string.Format("Server({0}) is unavailable due to high failure rate({1}%) and ping failure", server, failurePercent);
                    _logger.Warn(message, _loadBalancerContext.AdditionalInfo.Copy().With(AvailabilityKey, bool.FalseString));
                    var checkHealthEventMetric = GetCheckHealthEventMetric(route, server.ServerId);
                    checkHealthEventMetric.AddEvent("PullOut");
                }
                return pingSuccess;
            }
        }

        private bool CheckHealth(LoadBalancerRoute route)
        {
            bool statusChanged = false;
            Stopwatch watch = Stopwatch.StartNew();
            var additionInfo = _loadBalancerContext.AdditionalInfo.Copy().With("RouteId", route.RouteId);

            var servers = route.Servers;
            if (servers.Length == 0)
                return false;

            if (route.AvailableServers.Length == 0)
            {
                string message = "There is no available server in the list. Need check health.";
                _logger.Warn(message, additionInfo);
            }

            int checkedCount = 0, availableCount = 0;
            foreach (Server server in servers)
            {
                try
                {
                    bool isAlive = server.IsAlive;
                    if (isAlive != CheckHealth(route, server))
                    {
                        statusChanged = true;
                    }
                    if (server.IsAlive)
                        ++availableCount;
                    ++checkedCount;
                }
                catch (Exception ex)
                {
                    _logger.Warn("Error occurred while check server", ex, additionInfo);
                }
            }

            watch.Stop();
            var checkHealthLatencyMetric = GetCheckHealthLatencyMetric(route);
            checkHealthLatencyMetric.AddValue(watch.ElapsedMilliseconds);

            bool hasUnavailableServers = checkedCount != servers.Length || availableCount != servers.Length;
            if (hasUnavailableServers || _lastHasUnavailableServers)
            {
                string messageFormat = "{0} servers checked ({1}/{2}) in route:{3}";
                string message = string.Format(messageFormat, checkedCount, availableCount, servers.Length, route.RouteId);
                _logger.Info(message, additionInfo);
            }
            _lastHasUnavailableServers = hasUnavailableServers;
            
            return statusChanged;
        }

        public event EventHandler ServerStatusChange;

        private void RaisingServerStatusChangeEvent() 
        {
            if (ServerStatusChange == null)
                return;

            try
            {
                _logger.Info("Raising server status change event...", _loadBalancerContext.AdditionalInfo);
                ServerStatusChange(this, new EventArgs());
                _logger.Info("Server status change event is completed!", _loadBalancerContext.AdditionalInfo);
            }
            catch (Exception ex)
            {
                _logger.Warn("Error occurred while raising server status change event!", ex, _loadBalancerContext.AdditionalInfo);
            }
        }

        private IAuditMetric GetCheckHealthLatencyMetric(LoadBalancerRoute route)
        {
            var metricId = _loadBalancerContext.GetMetricId(route.RouteId + "." + CheckHealthLatencyMetricSuffix);
            var metricName = _loadBalancerContext.GetMetricName(CheckHealthLatencyMetricSuffix);
            var distributionMetricName = _loadBalancerContext.GetDistributionMetricName(CheckHealthLatencyMetricSuffix);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata["metric_name_audit"] = metricName;
            metadata["metric_name_distribution"] = distributionMetricName;
            metadata["loadbalancerid"] = _loadBalancerContext.LoadBalancerId;
            metadata["routeid"] = route.RouteId;
            return _checkHealthLatencyMetrics.GetOrAdd(route.RouteId, key => _loadBalancerContext.AuditMetricManager.GetMetric(metricId, new MetricConfig(metadata)));
        }

        private IEventMetric GetCheckHealthEventMetric(LoadBalancerRoute route, string serverId)
        {
            var metricId = _loadBalancerContext.GetMetricId(route.RouteId + "." + serverId + "." + CheckHealthEventMetricSuffix);
            var distributionMetricName = _loadBalancerContext.GetDistributionMetricName(CheckHealthEventMetricSuffix);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata["metric_name_distribution"] = distributionMetricName;
            metadata["loadbalancerid"] = _loadBalancerContext.LoadBalancerId;
            metadata["routeid"] = route.RouteId;
            metadata["serverid"] = serverId;
            return _checkHealthEventMetrics.GetOrAdd(route.RouteId, key => _loadBalancerContext.EventMetricManager.GetMetric(metricId, new MetricConfig(metadata)));
        }
    }
}
