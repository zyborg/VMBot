using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.GuacAPI.Model
{
    public class ConnectionHistory
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }
        
        [JsonPropertyName("connectionIdentifier")]
        public string ConnectionIdentifier { get; set; }
        
        [JsonPropertyName("connectionName")]
        public string ConnectionName { get; set; }
        
        [JsonPropertyName("startDate")]
        public long? StartDate { get; set; }
        
        [JsonPropertyName("endDate")]
        public long? EndDate { get; set; }
        
        [JsonPropertyName("remoteHost")]
        public string RemoteHost { get; set; }
        
        [JsonPropertyName("sharingProfileIdentifier")]
        public string SharingProfileIdentifier { get; set; }
        
        [JsonPropertyName("sharingProfileName")]
        public string SharingProfileName { get; set; }
        
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}