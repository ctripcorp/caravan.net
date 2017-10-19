using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Ribbon.Util;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Utility;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Collections.Generic;
using Com.Ctrip.Soa.Caravan.Ribbon.Algorithm;

using Math = Com.Ctrip.Soa.Caravan.Ribbon.Algorithm.Math;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Rule
{
    public class WeightedRoundRobinRule : RoundRobinRule
    {
        public override string Id
        {
            get { return DefaultLoadBalancerRuleFactoryManager.WeightedRoundRobinRuleName; }
        }

        public override string Description
        {
            get { return DefaultLoadBalancerRuleFactoryManager.WeightedRoundRobinRuleDescription; }
        }

        public WeightedRoundRobinRule()
        {
        }

        protected override List<ServerGroup> BuildLoadBalanceItems(LoadBalancerRoute route)
        {
            int weightGCD = GetWeightGCD(route.ServerGroups);

            var newLoadBalanceItems = new List<ServerGroup>();
            foreach (var serverGroup in route.ServerGroups)
            {
                int weight = serverGroup.Weight / weightGCD;
                for (int i = 0; i < weight; i++)
                {
                    newLoadBalanceItems.Add(serverGroup);
                }
            }
            newLoadBalanceItems.Shuffle();
            return newLoadBalanceItems;
        }

        private int GetWeightGCD(ServerGroup[] serverGroups) 
        {
            int weightGCD = 1;
            for (int i = 0; i < serverGroups.Length; i++)
            {
                int weight = serverGroups[i].Weight;
                if (weight <= 0)
                    continue;

                if (i == 0)
                    weightGCD = weight;
                else
                    weightGCD = Math.GCD(weightGCD, weight);
            }
            return weightGCD;
        }
    }
}
