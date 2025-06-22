using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Conversations;

public class ChatModel
{
    [JsonPropertyName("model")]
    public required string Model { get; set; }

    [JsonPropertyName("messages")]
    public required IDictionary<string, MessageModel> Messages { get; set; }

    [JsonPropertyName("temperature")]
    public required decimal Temperature { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream => false;
}
