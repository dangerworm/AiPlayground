using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Conversations
{
    public class EnvironmentSoundModel
    {
        [JsonPropertyName("content")]
        public required string Content { get; set; }

        [JsonPropertyName("source")]
        public required string Source { get; set; }

        [JsonPropertyName("type")]
        public required string Type { get; set; }
    }
}
