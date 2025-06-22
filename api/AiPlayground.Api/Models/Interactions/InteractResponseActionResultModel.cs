using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Interactions;

public class InteractResponseActionResultModel
{
    [JsonPropertyName("action_name")]
    public required string ActionName { get; set; }

    [JsonPropertyName("action_result")]
    public required string ActionResult { get; set; }
}
