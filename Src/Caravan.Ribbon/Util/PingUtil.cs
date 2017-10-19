using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Util
{
    internal class PingUtil
    {
        private ILog _logger;
        private ILoadBalancerContext _loadBalancerContext;

        public PingUtil(ILoadBalancerContext loadBalancerContext, ILog logger)
        {
            ParameterChecker.NotNull(loadBalancerContext, "loadBalancerContext");
            ParameterChecker.NotNull(logger, "logger");

            _loadBalancerContext = loadBalancerContext;
            _logger = logger;
        }

        public bool HasPing
        {
            get
            {
                return _loadBalancerContext.Ping != null;
            }
        }

        public bool IsAlive(Server server)
        {
            try
            {
                if (!HasPing)
                {
                    return true;
                }

                return _loadBalancerContext.Ping.IsAlive(server);
            }
            catch (Exception t)
            {
                var message = string.Format("Error occurred while ping {0}.", server);
                _logger.Warn(message, t, _loadBalancerContext.AdditionalInfo);
                return false;
            }
        }
    }
}
