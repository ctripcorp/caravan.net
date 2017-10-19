using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Com.Ctrip.Soa.Caravan.Collections.Generic;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    [DataContract]
    public class ServerGroup : IEquatable<ServerGroup>
    {
        [DataMember]
        public string GroupId { get; private set; }

        [DataMember]
        public int Weight { get; private set; }

        [DataMember]
        public Server[] Servers { get; private set; }

        [DataMember]
        public Dictionary<string, string> Metadata { get; private set; }

        public Server[] AvailableServers { get; private set; }

        public ServerGroup()
            : this(null, 0, null, null)
        { }

        public ServerGroup(string groupId, int weight)
            : this(groupId, weight, null, null)
        { }

        public ServerGroup(string groupId, int weight, IEnumerable<Server> servers)
            : this(groupId, weight, servers, null)
        { }

        public ServerGroup(string groupId, int weight, IEnumerable<Server> servers, Dictionary<string, string> metadata)
        {
            GroupId = groupId;
            Weight = weight;
            var serverList = new List<Server>();
            var availableServerList = new List<Server>();
            if (servers != null)
            {
                foreach (var server in servers)
                {
                    if (server == null || string.IsNullOrWhiteSpace(server.ServerId))
                        continue;
                    serverList.Add(server);
                    if (server.IsAlive)
                        availableServerList.Add(server);
                }
            }
            Servers = serverList.ToArray();
            AvailableServers = availableServerList.ToArray();
            Metadata = new Dictionary<string, string>();
            if (metadata != null)
                metadata.ForEach(item => Metadata[item.Key] = item.Value);
        }

        internal void RefreshAvailableServers()
        {
            AvailableServers = Servers.Where(server => server.IsAlive).ToArray();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ServerGroup);
        }

        public bool Equals(ServerGroup other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (!string.Equals(this.GroupId, other.GroupId, StringComparison.OrdinalIgnoreCase))
                return false;

            if (this.Weight != other.Weight)
                return false;

            if (this.Servers == null || other.Servers == null)
                return false;

            if (this.Servers.Length != other.Servers.Length)
                return false;

            if (this.Servers.Except(other.Servers).Any())
                return false;

            if (this.Metadata == null || this.Metadata.Count == 0)
                return other.Metadata == null || other.Metadata.Count == 0;

            if (other.Metadata == null || other.Metadata.Count == 0)
                return false;

            if (this.Metadata.Count != other.Metadata.Count)
                return false;

            return !this.Metadata.Except(other.Metadata).Any();
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(GroupId))
                return 0;
            return GroupId.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder serversBuilder = new StringBuilder();
            if (Servers == null)
                serversBuilder.Append("null");
            else
            {
                serversBuilder.Append("[");
                foreach (var server in Servers)
                {
                    serversBuilder.AppendFormat("{0}, ", server.ToString());
                }
                if (Servers.Length > 0)
                    serversBuilder.Remove(serversBuilder.Length - 2, 2);
                serversBuilder.Append("]");
            }

            StringBuilder metadataBuilder = new StringBuilder();
            if (Metadata == null)
                metadataBuilder.Append("null");
            else
            {
                metadataBuilder.Append("[");
                foreach (var item in Metadata)
                {
                    metadataBuilder.AppendFormat("{{\"{0}\": \"{1}\"}}, ", item.Key, item.Value);
                }
                if (Metadata.Count > 0)
                    metadataBuilder.Remove(metadataBuilder.Length - 2, 2);
                metadataBuilder.Append("]");
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{{\"GroupId\": \"{0}\", \"Weight\": {1}, \"Servers\":{2}, \"metadata\": {3}}}", GroupId, Weight, serversBuilder, metadataBuilder);
            return builder.ToString();
        }
    }
}
