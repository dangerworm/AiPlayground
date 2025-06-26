using System.Text.Json.Serialization;

namespace AiPlayground.Core.Models.Conversations;

public class OllamaInputModel
{
    [JsonPropertyName("model")]
    public required string Model { get; set; }

    [JsonPropertyName("messages")]
    public required IEnumerable<MessageModel> Messages { get; set; }

    [JsonPropertyName("temperature")]
    public required decimal Temperature { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream => false;
}
