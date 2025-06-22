using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Interactions;

public class InteractResponseSoundModel
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("source")]
    public required string Source { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }
}
