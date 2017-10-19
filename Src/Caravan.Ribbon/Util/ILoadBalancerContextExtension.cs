using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Util
{
    static class ILoadBalancerContextExtension
    {
        public static string GetMetricId(this ILoadBalancerContext loadBalancerContext, string metricSuffix)
        {
            return string.Format("{0}.{1}", loadBalancerContext.LoadBalancerKey, metricSuffix);
        }

        public static string GetMetricName(this ILoadBalancerContext loadBalancerContext, string metricSuffix)
        {
            return string.Format("{0}.{1}", loadBalancerContext.ManagerId, metricSuffix);
        }

        public static string GetDistributionMetricName(this ILoadBalancerContext loadBalancerContext, string metricSuffix)
        {
            return string.Format("{0}.{1}.distribution", loadBalancerContext.ManagerId, metricSuffix);
        }
    }
}
