using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    public class LoadBalancerRouteConfig : IEquatable<LoadBalancerRouteConfig>, IComparable<LoadBalancerRouteConfig>
    {
        public string RouteId { get; private set; }

        public int Priority { get; private set; }
        
        public bool AllowFallback { get; private set; }

        public LoadBalancerRouteConfig(string routeId, int priority, bool allowFallback)
        {
            RouteId = routeId;
            Priority = priority;
            AllowFallback = allowFallback;
        }

        public bool Equals(LoadBalancerRouteConfig other)
        {
            if (other == null)
                return false;
            return string.Equals(this.RouteId, other.RouteId, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(LoadBalancerRouteConfig other)
        {
            if (other == null)
                return 1;

            return other.Priority.CompareTo(this.Priority);
        }

        public override string ToString()
        {
            return "{\"RouteId\":\"" + RouteId + "\", \"Priority\":\"" + Priority + "\",\"AllowFallback\":\"" + AllowFallback + "\"}";
        }
    }
}
