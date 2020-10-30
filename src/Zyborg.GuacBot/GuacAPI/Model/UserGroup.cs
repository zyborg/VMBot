using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.GuacAPI.Model
{
    public class UserGroup
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        public List<string> MemberUsers { get; set; }
        public List<string> ParentUserGroups { get; set; }
        public List<string> MemberUserGroups { get; set; }
        public Dictionary<string, object> Permissions { get; set; }
    }

    [Flags]
    public enum UserGroupDetails
    {
        MemberUsers = 1,
        ParentUserGroups = 2,
        MemberUserGroups = 4,
        Permissions = 8,
    }
}