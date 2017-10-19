using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Utility;
using Com.Ctrip.Soa.Caravan.Ribbon.Rule;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    public class LoadBalancerConfig
    {
        public Func<ILoadBalancerRule> RuleFactory { get; private set; }
        public IPing Ping { get; private set; }
        public IServerSource ServerSource { get; private set; }
        public EventHandler<ServerSourceRestoreEvent> ServerSourceRestoreEventHandler { get; set; }
        public EventHandler<SeekRouteEventArgs> SeekRouteEventHandler { get; set; }

        public LoadBalancerConfig(IServerSource serverSource)
            : this(null, serverSource)
        { }

        public LoadBalancerConfig(IPing ping, IServerSource serverSource)
            : this(null, ping, serverSource)
        { }

        public LoadBalancerConfig(Func<ILoadBalancerRule> ruleFactory, IPing ping, IServerSource serverSource)
        {
            ParameterChecker.NotNull(serverSource, "serverSource");

            this.RuleFactory = ruleFactory;
            this.Ping = ping;
            this.ServerSource = serverSource;
        }
    }
}
