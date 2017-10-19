using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Metric;

namespace Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer
{
    interface ILoadBalancerContext
    {
        string ManagerId { get; }

        string LoadBalancerId { get; }

        string LoadBalancerKey { get; }

        ILoadBalancerRule GetLoadBalancerRule(string ruleId);

        IPing Ping { get; }

        ILoadBalancer LoadBalancer { get; }

        IServerSource ServerSource { get; }

        IServerSourceFilter ServerSourceFilter { get; }

        IServerSourceManager ServerSourceManager { get; }

        IServerSourceMonitor ServerSourceMonitor { get; }

        IConfigurationManager ConfigurationManager { get; }

        Dictionary<string, string> AdditionalInfo { get; }

        IEventMetricManager EventMetricManager { get; }

        IAuditMetricManager AuditMetricManager { get; }

        IStatusMetricManager<double> StatusMetricManager { get; }

        ServerStats GetServerStats(Server server);

        int MinAvailableServerCount { get; }
    }
}
