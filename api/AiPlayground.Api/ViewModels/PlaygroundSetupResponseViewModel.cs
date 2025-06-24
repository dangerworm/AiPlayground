using System.Text.Json.Serialization;

namespace AiPlayground.Api.ViewModels
{
    public class PlaygroundSetupResponseViewModel
    {
        [JsonPropertyName("available_models")]
        public required IList<string> AvailableModels { get; set; }

        [JsonPropertyName("characters")]
        public required IList<CharacterViewModel> Characters { get; set; }

        [JsonPropertyName("cell_size")]
        public required int CellSize { get; set; }

        [JsonPropertyName("grid_width")]
        public required int GridWidth { get; set; }

        [JsonPropertyName("grid_height")]
        public required int GridHeight { get; set; }
    }
}
