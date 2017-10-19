using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    public static class ConfigurationKeys
    {
        public const string DataFolder = "ribbon.local-cache.data-folder";

        public const string CheckInterval = "ribbon.check-interval-in-millisecond";

        public const string FailureThresholdPercentage = "ribbon.failure-threshold-percentage";

        public const string CounterBuffer = "ribbon.counter-buffer";

        public const string LoadBalancerRuleName = "ribbon.lb.rule.name";

        public const string MinAvailableServerCount = "ribbon.min-available-server-count";
    }
}
