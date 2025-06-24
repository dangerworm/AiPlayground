using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Conversations
{
    public class CharacterActionModel
    {
        [JsonPropertyName("action_name")]
        public required string ActionName { get; set; }

        [JsonPropertyName("action_result")]
        public required string ActionResult { get; set; }
    }
}
