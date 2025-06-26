using System.Text.Json.Serialization;

namespace AiPlayground.Api.ViewModels
{
    public class QuestionViewModel
    {
        [JsonPropertyName("character_id")]
        public required Guid CharacterId { get; set; }

        [JsonPropertyName("question")]
        public required string Question { get; set; }
    }
}
