using System.Text.Json.Serialization;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Api.Models.Playground
{
    public class PlaygroundSetupResponseModel
    {
        [JsonPropertyName("available_models")]
        public required IEnumerable<string> AvailableModels { get; set; }

        [JsonPropertyName("characters")]
        public required IEnumerable<CharacterDto> Characters { get; set; }

        [JsonPropertyName("cell_size")]
        public required int CellSize { get; set; }

        [JsonPropertyName("grid_width")]
        public required int GridWidth { get; set; }

        [JsonPropertyName("grid_height")]
        public required int GridHeight { get; set; }
    }
}
