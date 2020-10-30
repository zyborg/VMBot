using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.DuoMFA.Model
{
    public partial class FrameStatusResult
    {
        [JsonPropertyName("stat")]
        public string Stat { get; set; }

        [JsonPropertyName("response")]
        public ResponseDetail Response { get; set; }

        public partial class ResponseDetail
        {
            [JsonPropertyName("parent")]
            public string Parent { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("result")]
            public string Result { get; set; }

            [JsonPropertyName("status_code")]
            public string StatusCode { get; set; }

            [JsonPropertyName("reason")]
            public string Reason { get; set; }

            [JsonPropertyName("result_url")]
            public string ResultUrl { get; set; }
        }
    }
}