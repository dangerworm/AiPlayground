using System.Text.Json.Serialization;

namespace AiPlayground.Api.ViewModels
{
    public class PlaygroundViewModel
    {
        [JsonPropertyName("available_models")]
        public required IEnumerable<string> AvailableModels { get; set; }

        [JsonPropertyName("characters")]
        public required IEnumerable<CharacterViewModel> Characters { get; set; }

        [JsonPropertyName("cell_size")]
        public required int CellSize { get; set; }

        [JsonPropertyName("grid_width")]
        public required int GridWidth { get; set; }

        [JsonPropertyName("grid_height")]
        public required int GridHeight { get; set; }

        [JsonPropertyName("questions")]
        public IEnumerable<QuestionViewModel>? Questions { get; set; }
    }
}
