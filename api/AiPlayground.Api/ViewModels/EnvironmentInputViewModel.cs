using System.Text.Json.Serialization;
using AiPlayground.Core.Models.Conversations;

namespace AiPlayground.Api.ViewModels;

public class EnvironmentInputViewModel(EnvironmentInputModel environmentInputModel)
{
    [JsonPropertyName("correlation_id")]
    public Guid? CorrelationId { get; set; } = environmentInputModel.CorrelationId;

    [JsonPropertyName("action_results")]
    public IEnumerable<EnvironmentActionResultViewModel> ActionResults { get; set; } = environmentInputModel.ActionResults.Select(ar => new EnvironmentActionResultViewModel(ar));

    [JsonPropertyName("age")]
    public int Age { get; set; } = environmentInputModel.Age;

    [JsonPropertyName("environment")]
    public string Environment { get; set; } = environmentInputModel.Environment;

    [JsonPropertyName("grid_position")]
    public string GridPosition { get; set; } = environmentInputModel.GridPosition;

    [JsonPropertyName("iteration")]
    public int Iteration { get; set; } = environmentInputModel.Iteration;

    [JsonPropertyName("sounds")]
    public IEnumerable<EnvironmentSoundViewModel> Sounds { get; set; } = environmentInputModel.Sounds.Select(s => new EnvironmentSoundViewModel(s));

    [JsonPropertyName("date_time_iso8601")]
    public string DateTimeIso8601 { get; set; } = environmentInputModel.DateTimeIso8601;
}
