using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    public interface ILoadBalancerRule
    {
        string Id { get; }

        string Description { get; }

        Server Choose(LoadBalancerRoute route);
    }
}
