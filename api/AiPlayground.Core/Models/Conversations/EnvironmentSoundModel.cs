using System.Text.Json.Serialization;

namespace AiPlayground.Core.Models.Conversations;

public class EnvironmentSoundModel
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("source")]
    public required string Source { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }
}
