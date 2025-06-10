using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Interactions
{
    public class InteractResponseModel
    {
        [JsonPropertyName("action_results")]
        public IEnumerable<InteractResponseActionResultModel>? ActionResults { get; set; }

        [JsonPropertyName("age")]
        public required int Age { get; set; }

        [JsonPropertyName("environment")]
        public required string Environment { get; set; }

        [JsonPropertyName("position")]
        public required int[] Position { get; set; }

        [JsonPropertyName("sound")]
        public IEnumerable<InteractResponseSoundModel>? Sound { get; set; }

        [JsonPropertyName("time")]
        public required int Time { get; set; }
    }
}
