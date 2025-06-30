using System.Text.Json.Serialization;
using AiPlayground.Core.Constants;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Api.ViewModels
{
    public class CharacterViewModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; }

        [JsonPropertyName("age")]
        public int Age { get; }

        [JsonPropertyName("colour")]
        public string Colour { get; }

        [JsonPropertyName("grid_position")]
        public Tuple<int, int> GridPosition { get; }

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("responses")]
        public IEnumerable<CharacterResponseViewModel> Responses { get; }

        [JsonPropertyName("questions")]
        public IEnumerable<string> Questions { get; }

        public CharacterViewModel(CharacterDto character)
        {
            Id = character.Id;
            CreatedAt = character.CreatedAt;
            Age = character.AgeInIterations;
            Colour = character.Colour;
            GridPosition = character.GridPosition;
            Model = "GPT 4.1";
            Responses = [.. character.Responses.Select(r => new CharacterResponseViewModel(r))];
            Questions = character.Questions;
        }
    }
}
