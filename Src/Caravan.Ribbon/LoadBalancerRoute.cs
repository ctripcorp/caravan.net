using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Collections.Generic;
using Com.Ctrip.Soa.Caravan.Utility;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    [DataContract]
    public class LoadBalancerRoute : IEquatable<LoadBalancerRoute>
    {
        [DataMember]
        public string RouteId { get; private set; }

        [DataMember]
        public ServerGroup[] ServerGroups { get; private set; }

        private List<Server> servers;
        public Server[] Servers
        {
            get
            {
                if (servers == null)
                    BuildFields();
                return servers.ToArray();
            }
        }

        public Server[] AvailableServers
        {
            get
            {
                var newAvailableServers = new List<Server>();
                Servers.ForEach(server =>
                {
                    if (server.IsAlive)
                        newAvailableServers.Add(server);
                });
                return newAvailableServers.ToArray();
            }
        }

        private Dictionary<string, ServerGroup> serverGroupMap;
        public Dictionary<string, ServerGroup> ServerGroupMap
        {
            get
            {
                if (serverGroupMap == null)
                    BuildFields();
                return serverGroupMap;
            }
        }

        public LoadBalancerRoute() : this(null) { }

        public LoadBalancerRoute(string routeId) 
        { 
            RouteId = routeId;
            ServerGroups = new ServerGroup[0];
        }

        public LoadBalancerRoute(string routeId, IEnumerable<ServerGroup> serverGroups) 
        {
            ParameterChecker.NotNullOrWhiteSpace(routeId, "routeId");
            ParameterChecker.NotNull(serverGroups, "serverGroups");

            RouteId = routeId;
            ServerGroups = serverGroups.Where(group => group != null && !string.IsNullOrWhiteSpace(group.GroupId)).ToArray();
        }

        private void BuildFields()
        {
            var newServers = new List<Server>();
            var newServerGroupMap = new Dictionary<string,ServerGroup>(StringComparer.OrdinalIgnoreCase);
            if (ServerGroups == null)
                ServerGroups = new ServerGroup[0];
            ServerGroups.ForEach(serverGroup => 
            {
                newServers.AddRange(serverGroup.Servers);
                newServerGroupMap[serverGroup.GroupId] = serverGroup;
            });

            servers = newServers;
            serverGroupMap = newServerGroupMap;
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(RouteId))
                return 0;
            return RouteId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LoadBalancerRoute);
        }

        public bool Equals(LoadBalancerRoute other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (!string.Equals(this.RouteId, other.RouteId, StringComparison.OrdinalIgnoreCase))
                return false;

            if (this.ServerGroupMap == null || this.ServerGroupMap.Count == 0)
                return other.ServerGroupMap == null || other.ServerGroupMap.Count == 0;

            if (other.ServerGroupMap == null || other.ServerGroupMap.Count == 0)
                return false;

            if (this.ServerGroupMap.Count != other.ServerGroupMap.Count)
                return false;

            return !this.ServerGroupMap.Except(other.ServerGroupMap).Any();
        }

        public override string ToString()
        {
            string routeId = RouteId ?? "null";
            string serverGroups;
            if (ServerGroups == null)
                serverGroups = "null";
            else
                serverGroups = string.Format("[{0}]", string.Join<ServerGroup>(",", ServerGroups));
            return "{\"RouteId\":\"" + routeId + "\", \"ServerGroups\":" + serverGroups + "}";
        }
    }
}
