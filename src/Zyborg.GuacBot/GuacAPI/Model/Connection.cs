using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.GuacAPI.Model
{
    public partial class Connection
    {
        [JsonPropertyName("name")]
        public string Username { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("parentIdentifier")]
        public string ParentIdentifier { get; set; }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("activeConnections")]
        public int ActiveConnections { get; set; }

        [JsonPropertyName("lastActive")]
        public long LastActive { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
        public List<ConnectionHistory> History { get; set; }

    }

    [Flags]
    public enum ConnectionDetails
    {
        Parameters = 1,
        History = 2,
    }
}
