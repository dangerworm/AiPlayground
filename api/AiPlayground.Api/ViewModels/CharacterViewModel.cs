using System.Text.Json.Serialization;
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
        public IList<CharacterResponseViewModel> Responses { get; }

        [JsonPropertyName("questions")]
        public IList<string> Questions { get; }

        public CharacterViewModel(CharacterDto character)
        {
            Id = character.Id;
            CreatedAt = character.CreatedAt;
            Age = character.AgeInIterations;
            Colour = character.Colour;
            GridPosition = character.GridPosition;
            Model = character.Connection.Model;
            Responses = [.. character.Responses.Select(r => new CharacterResponseViewModel(r))];
            Questions = character.Questions;
        }
    }
}
