using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.GuacAPI.Model
{
    public partial class ConnectionGroup
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("parentIdentifier")]
        public string ParentIdentifier { get; set; }

        [JsonPropertyName("type")]
        public ConnectionGroupType? Type { get; set; }

        [JsonPropertyName("activeConnections")]
        public int? ActiveConnections { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        [JsonPropertyName("childConnectionGroups")]
        public List<ConnectionGroup> ChildConnectionGroups { get; set; }

        [JsonPropertyName("childConnections")]
        public List<Connection> ChildConnections { get; set; }
    }

    public enum ConnectionGroupType
    {
        [DefaultValue("ORGANIZATIONAL")]
        Organizational,
        [DefaultValue("BALANCING")]
        Balancing,
    }
}
