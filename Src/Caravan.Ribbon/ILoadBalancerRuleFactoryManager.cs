using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    interface ILoadBalancerRuleFactoryManager
    {
        ILoadBalancerRule GetLoadBalancerRule(string ruleId);

        Func<ILoadBalancerRule> GetLoadBalancerRuleFactory();

        void RegisterLoadBalancerRuleFactory(Func<ILoadBalancerRule> rule);
    }
}
