using System.Text.Json.Serialization;

namespace AiPlayground.Api.ViewModels;

public class PlaygroundViewModel
{
    [JsonPropertyName("available_models")]
    public required IEnumerable<string> AvailableModels { get; set; }

    [JsonPropertyName("characters")]
    public required IEnumerable<CharacterViewModel> Characters { get; set; }

    [JsonPropertyName("grid_width")]
    public required int GridWidth { get; set; }

    [JsonPropertyName("grid_height")]
    public required int GridHeight { get; set; }

    [JsonPropertyName("iteration")]
    public required int Iteration { get; set; }
}
