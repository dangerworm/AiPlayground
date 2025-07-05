using System.Text.Json.Serialization;
using AiPlayground.Core.Models.Conversations;

namespace AiPlayground.Api.ViewModels;

public class EnvironmentSoundViewModel
{
    public EnvironmentSoundViewModel(EnvironmentSoundModel environmentSoundModel)
    {
        Content = environmentSoundModel.Content;
        Source = environmentSoundModel.Source;
        Type = environmentSoundModel.Type;
    }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}
