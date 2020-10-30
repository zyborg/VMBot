using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.GuacAPI.Model
{
    public partial class User
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        public List<object> History { get; set; }
        public List<object> UserGroups { get; set; }
        public Dictionary<string, object> Permissions { get; set; }
        public Dictionary<string, object> EffectivePermissions { get; set; }
    }

    [Flags]
    public enum UserDetails
    {
        History = 1,
        UserGroups = 2,
        Permissions = 4,
        EffectivePermissions = 8,
    }
}
