using System.Text.Json.Serialization;

namespace AiPlayground.Core.Models.Conversations;

public class MessageModel
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;
    
    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;

    public override string ToString()
    {
        return $"Role: {Role}{Environment.NewLine}Content: {Content}";
    }
}
