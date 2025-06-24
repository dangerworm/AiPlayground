using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Conversations
{
    public class CharacterResponseModel
    {
        [JsonPropertyName("decisions")]
        public IList<string>? Decisions { get; set; } = [];

        [JsonPropertyName("desires")]
        public IList<string>? Desires { get; set; } = [];

        [JsonPropertyName("emotion")]
        public string? Emotion { get; set; } = string.Empty;

        [JsonPropertyName("thoughts")]
        public string? Thoughts { get; set; } = string.Empty;
    }
}
