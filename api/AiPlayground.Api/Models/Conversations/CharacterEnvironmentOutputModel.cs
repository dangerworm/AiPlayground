using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Conversations
{
    public class CharacterEnvironmentOutputModel
    {
        [JsonPropertyName("action_results")]
        public IDictionary<string, string> ActionResults { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("age")]
        public required int Age { get; set; }

        [JsonPropertyName("environment")]
        public required string Environment { get; set; }

        [JsonPropertyName("grid_position")]
        public required string GridPosition { get; set; }

        [JsonPropertyName("iteration")]
        public required int Iteration { get; set; }

        [JsonPropertyName("sound")]
        public IDictionary<string, EnvironmentSoundModel> Sound { get; set; } = new Dictionary<string, EnvironmentSoundModel>();
    }
}
