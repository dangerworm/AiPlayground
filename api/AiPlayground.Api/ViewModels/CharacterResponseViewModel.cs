using System.Text.Json.Serialization;
using AiPlayground.Core.Models.Conversations;

namespace AiPlayground.Api.ViewModels;

public class CharacterResponseViewModel
{
    [JsonPropertyName("decisions")]
    public IEnumerable<string>? Decisions { get; set; } = [];

    [JsonPropertyName("desires")]
    public IEnumerable<string>? Desires { get; set; } = [];

    [JsonPropertyName("emotion")]
    public string? Emotion { get; set; } = string.Empty;

    [JsonPropertyName("thoughts")]
    public string? Thoughts { get; set; } = string.Empty;

    public CharacterResponseViewModel(CharacterResponseModel response)
    {
        Decisions = response.Decisions;
        Desires = response.Desires;
        Emotion = response.Emotion;
        Thoughts = response.Thoughts;
    }
}
