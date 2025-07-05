using System.Text.Json.Serialization;
using AiPlayground.Core.Models.Conversations;

namespace AiPlayground.Api.ViewModels;

public class CharacterResponseViewModel(CharacterResponseModel response)
{
    [JsonPropertyName("decisions")]
    public IEnumerable<string>? Decisions { get; set; } = response.Decisions;

    [JsonPropertyName("desires")]
    public IEnumerable<string>? Desires { get; set; } = response.Desires;

    [JsonPropertyName("emotion")]
    public string? Emotion { get; set; } = response.Emotion;

    [JsonPropertyName("thoughts")]
    public string? Thoughts { get; set; } = response.Thoughts;
}
