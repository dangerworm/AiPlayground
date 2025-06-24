using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Conversations;

public class MessageModel
{
    [JsonPropertyName("role")]
    public required string Role { get; set; }
    
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    public override string ToString()
    {
        return $"Role: {Role}{Environment.NewLine}Content: {Content}";
    }
}
