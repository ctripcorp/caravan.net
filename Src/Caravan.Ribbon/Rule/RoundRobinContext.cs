using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Ribbon.Algorithm;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Rule
{
    class RoundRobinContext
    {
        private IRoundRobinAlgorithm<Server> roundRobinAlgorithm;
        public ServerGroup ServerGroup { get; private set; }

        public RoundRobinContext(ServerGroup serverGroup)
        {
            ServerGroup = serverGroup;
            roundRobinAlgorithm = new DefaultRoundRobinAlgorithm<Server>();
        }

        public Server Choose()
        {
            return roundRobinAlgorithm.Choose(ServerGroup.AvailableServers);
        }
    }
}
