using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    [DataContract]
    public class Server : IEquatable<Server>
    {
        [DataMember]
        public string ServerId { get; private set; }

        [DataMember]
        public bool IsAlive { get; internal set; }

        [DataMember]
        public Dictionary<string, string> Metadata { get; private set; }

        public Server()
            : this(null, true, null)
        { }

        public Server(string id)
            : this(id, true, null)
        { }

        public Server(string id, bool isAlive)
            : this(id, isAlive, null)
        { }

        public Server(string id, Dictionary<string, string> metadata)
            : this(id, true, metadata)
        { }

        public Server(string id, bool isAlive, Dictionary<string, string> metadata)
        {
            this.IsAlive = isAlive;
            this.ServerId = id;
            this.Metadata = metadata;
        }

        public bool Equals(Server other)
        {
            if (other == null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (!string.Equals(this.ServerId, other.ServerId, StringComparison.OrdinalIgnoreCase))
                return false;

            if (this.Metadata == null || this.Metadata.Count == 0)
                return other.Metadata == null || other.Metadata.Count == 0;

            if (other.Metadata == null || other.Metadata.Count == 0)
                return false;

            if (this.Metadata.Count != other.Metadata.Count)
                return false;

            return !this.Metadata.Except(other.Metadata).Any();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Server);
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(ServerId))
                return 0;
            return ServerId.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder metadataBuilder = new StringBuilder();
            if (Metadata == null)
                metadataBuilder.Append("null");
            else
            {
                metadataBuilder.Append("[");
                foreach (var item in Metadata)
                {
                    metadataBuilder.AppendFormat("{{\"{0}\", \"{1}\"}}, ", item.Key, item.Value);
                }
                if (Metadata.Count > 0)
                    metadataBuilder.Remove(metadataBuilder.Length - 2, 2);
                metadataBuilder.Append("]");
            }
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{{\"id\": \"{0}\", \"isAlive\": {1}, \"metadata\": {2}}}", ServerId, IsAlive, metadataBuilder);
            return builder.ToString();
        }
    }
}
