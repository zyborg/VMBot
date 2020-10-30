using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.GuacAPI.Model
{
    public class JsonPatchValue<T>
    {
        public static readonly IEnumerable<JsonPatchValue<T>> Empty = new JsonPatchValue<T>[0];

        [JsonPropertyName("op")]
        public string Op { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("value")]
        public T Value { get; set; }
    }
}