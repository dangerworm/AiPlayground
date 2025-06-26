using System.Text.Json.Serialization;

namespace AiPlayground.Core.Models.Conversations;

public class CharacterResponseModel : ICorrelated
{
    [JsonPropertyName("correlation_id")]
    public Guid? CorrelationId { get; set; }

    [JsonPropertyName("decisions")]
    public IEnumerable<string>? Decisions { get; set; } = [];

    [JsonPropertyName("desires")]
    public IEnumerable<string>? Desires { get; set; } = [];

    [JsonPropertyName("emotion")]
    public string? Emotion { get; set; } = string.Empty;

    [JsonPropertyName("thoughts")]
    public string? Thoughts { get; set; } = string.Empty;
}
