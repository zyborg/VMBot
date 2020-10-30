using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.DuoMFA.Model
{
    public partial class FramePromptResult
    {
        [JsonPropertyName("stat")]
        public string Stat { get; set; }

        [JsonPropertyName("response")]
        public ResponseDetail Response { get; set; }

        public partial class ResponseDetail
        {
            [JsonPropertyName("txid")]
            public string Txid { get; set; }
        }
    }
}