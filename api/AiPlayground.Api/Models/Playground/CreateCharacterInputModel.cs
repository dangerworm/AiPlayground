using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Playground
{
    public class CreateCharacterInputModel
    {
        [JsonPropertyName("colour")]
        public required string Colour { get; init; }

        [JsonPropertyName("grid_position")]
        public required Tuple<int, int> GridPosition { get; init; }

        [JsonPropertyName("model")]
        public required string Model { get; init; }
    }
}
