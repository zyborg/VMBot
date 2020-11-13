using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zyborg.GuacBot.Model
{
    public class RebuildInstancesCustomEvent
    {
        [JsonProperty(nameof(CustomEventName))]
        public string CustomEventName { get; set; }

        [JsonProperty(nameof(State))]
        public string State { get; set; } = "pending";

        [JsonProperty(nameof(IncludeIds))]
        public string[] IncludeIds { get; set; }

        [JsonProperty(nameof(ExcludeIds))]
        public string[] ExcludeIds { get; set; }

        [JsonProperty(nameof(IncludeNameRegex))]
        public string[] IncludeNameRegex { get; set; }

        [JsonProperty(nameof(ExcludeNameRegex))]
        public string[] ExcludeNameRegex { get; set; }

        public static bool Matcher(JToken jtoken) =>
            jtoken.SelectToken(nameof(CustomEventName))?.ToString() == "RebuildInstances";
    }
}