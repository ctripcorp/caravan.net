using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Collections.Generic;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Util
{
    static class LoadBalancerRoutes
    {
        public static List<LoadBalancerRoute> FilterInvalidEntities(this List<LoadBalancerRoute> routes, ILog logger, Dictionary<string, string> tags)
        {
            if (routes.IsNullOrEmpty())
                return routes;

            List<LoadBalancerRoute> result = new List<LoadBalancerRoute>();
            foreach (var route in routes)
            {
                if (route == null)
                {
                    logger.Info("Route is null. Ignored.", tags);
                    continue;
                }
                if (string.IsNullOrWhiteSpace(route.RouteId))
                {
                    logger.Info("Route id is null or white space. Ignored.", tags);
                    continue;
                }

                List<ServerGroup> serverGroups = new List<ServerGroup>();
                foreach (var serverGroup in route.ServerGroups ?? new ServerGroup[0])
                {
                    if (serverGroup == null)
                    {
                        logger.Info("ServerGroup is null. Ignored.", tags);
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(serverGroup.GroupId))
                    {
                        logger.Info("ServerGroup id is null or white space. Ignored.", tags);
                        continue;
                    }

                    List<Server> servers = new List<Server>();
                    foreach (var server in serverGroup.Servers ?? new Server[0])
                    {
                        if (server == null)
                        {
                            logger.Info("Server is null. Ignored.", tags);
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(server.ServerId))
                        {
                            logger.Info("Server id is null or white space. Ignored.", tags);
                            continue;
                        }

                        servers.Add(server);
                    }

                    serverGroups.Add(new ServerGroup(serverGroup.GroupId, serverGroup.Weight, servers, serverGroup.Metadata));
                }

                result.Add(new LoadBalancerRoute(route.RouteId, serverGroups));
            }
            return result;
        }
    }
}
