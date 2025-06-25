using System.Text.Json.Serialization;

namespace AiPlayground.Core.Models.Conversations;

public class EnvironmentActionResultModel
{
    [JsonPropertyName("action_name")]
    public required string ActionName { get; set; }

    [JsonPropertyName("action_result")]
    public required string ActionResult { get; set; }
}
