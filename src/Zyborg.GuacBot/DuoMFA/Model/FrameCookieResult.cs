using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.DuoMFA.Model
{
    public partial class FrameCookieResult
    {
        [JsonPropertyName("stat")]
        public string Stat { get; set; }

        [JsonPropertyName("response")]
        public ResponseDetail Response { get; set; }

        public partial class ResponseDetail
        {
            [JsonPropertyName("parent")]
            public string Parent { get; set; }

            [JsonPropertyName("cookie")]
            public string Cookie { get; set; }
        }
    }
}