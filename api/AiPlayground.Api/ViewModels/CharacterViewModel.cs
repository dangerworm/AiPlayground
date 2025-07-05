using System.Text.Json.Serialization;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Api.ViewModels;

public class CharacterViewModel(CharacterDto character)
{
    [JsonPropertyName("id")]
    public Guid Id { get; } = character.Id;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; } = character.CreatedAt;

    [JsonPropertyName("age")]
    public int Age { get; } = character.AgeInIterations;

    [JsonPropertyName("name")]
    public string Name { get; } = character.Name;

    [JsonPropertyName("colour")]
    public string Colour { get; } = character.Colour;

    [JsonPropertyName("grid_position")]
    public Tuple<int, int> GridPosition { get; } = character.GridPosition;

    [JsonPropertyName("model")]
    public string Model { get; } = "GPT 4.1";

    [JsonPropertyName("inputs")]
    public IEnumerable<EnvironmentInputViewModel> Inputs { get; } = [.. character.Inputs.Select(i => new EnvironmentInputViewModel(i))];

    [JsonPropertyName("responses")]
    public IEnumerable<CharacterResponseViewModel> Responses { get; } = [.. character.Responses.Select(r => new CharacterResponseViewModel(r))];

    [JsonPropertyName("questions")]
    public IEnumerable<QuestionAnswerViewModel> Questions { get; } = character.Questions.Select(q => new QuestionAnswerViewModel(q));
}
