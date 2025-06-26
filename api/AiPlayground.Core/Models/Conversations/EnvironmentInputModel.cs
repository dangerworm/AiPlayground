using System.Text.Json.Serialization;

namespace AiPlayground.Core.Models.Conversations;

public class EnvironmentInputModel : ICorrelated
{
    [JsonPropertyName("correlation_id")]
    public Guid? CorrelationId { get; set; }

    [JsonPropertyName("action_results")]
    public IEnumerable<EnvironmentActionResultModel> ActionResults { get; set; } = [];

    [JsonPropertyName("age")]
    public required int Age { get; set; }

    [JsonPropertyName("environment")]
    public required string Environment { get; set; }

    [JsonPropertyName("grid_position")]
    public required string GridPosition { get; set; }

    [JsonPropertyName("iteration")]
    public required int Iteration { get; set; }

    [JsonPropertyName("sounds")]
    public IEnumerable<EnvironmentSoundModel> Sounds { get; set; } = [];

    [JsonPropertyName("time")]
    public required int Time { get; set; }
}
