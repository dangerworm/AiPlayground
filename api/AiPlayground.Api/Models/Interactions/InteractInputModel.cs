using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Interactions;

public class InteractInputModel
{
    [JsonPropertyName("character_id")]
    public required Guid CharacterId { get; set; }
}
