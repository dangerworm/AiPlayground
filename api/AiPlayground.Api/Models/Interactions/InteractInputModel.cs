using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Interactions;

public class InteractInputModel
{
    [JsonPropertyName("character_id")]
    public required Guid CharacterId { get; set; }

    [JsonPropertyName("decisions")]
    public string[] Decisions { get; set; } = [];

    [JsonPropertyName("desires")]
    public string[] Desires { get; set; } = [];

    [JsonPropertyName("emotion")]
    public string Emotion { get; set; } = string.Empty;

    [JsonPropertyName("thoughts")]
    public string Thoughts { get; set; } = string.Empty;
}
