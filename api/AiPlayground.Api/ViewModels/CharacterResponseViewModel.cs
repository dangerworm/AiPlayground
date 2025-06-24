using System.Text.Json.Serialization;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Api.ViewModels;

public class CharacterResponseViewModel
{
    [JsonPropertyName("decisions")]
    public IList<string> Decisions { get; set; } = [];

    [JsonPropertyName("desires")]
    public IList<string> Desires { get; set; } = [];

    [JsonPropertyName("emotion")]
    public string Emotion { get; set; } = string.Empty;

    [JsonPropertyName("thoughts")]
    public string Thoughts { get; set; } = string.Empty;

    public CharacterResponseViewModel(CharacterResponseDto response)
    {
        Decisions = response.Decisions;
        Desires = response.Desires;
        Emotion = response.Emotion;
        Thoughts = response.Thoughts;
    }
}
