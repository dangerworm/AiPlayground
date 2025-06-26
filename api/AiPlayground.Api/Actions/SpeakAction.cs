using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AiPlayground.Api.Attributes;
using AiPlayground.Core.Constants;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Actions;

public class SpeakAction(CharacterRepository characterRepository) : ActionBase, IAction
{
    private CharacterRepository _characterRepository = characterRepository;

    public override string Description => "Speak the string aloud, projecting to a radius of 'projection' across the grid.";

    [JsonPropertyName("message")]
    [ExampleValue("Hello, nice to meet you.")]
    public required string Message { get; set; }

    [JsonPropertyName("projection")]
    [ExampleValue(2)]
    public required int Projection { get; set; }

    public async Task<string> Run(Guid characterId)
    {
        var character = await _characterRepository.GetCharacterByIdAsync(characterId);

        var projectionMinX = Math.Max(0, character.GridPosition.Item1 - Projection);
        var projectionMaxX = Math.Min(PlaygroundConstants.DefaultGridSize - 1, character.GridPosition.Item1 + Projection);

        var projectionMinY = Math.Max(0, character.GridPosition.Item2 - Projection);
        var projectionMaxY = Math.Min(PlaygroundConstants.DefaultGridSize - 1, character.GridPosition.Item2 + Projection);

        var projectionBox = $"from ({projectionMinX}, {projectionMinY}) to ({projectionMaxX}, {projectionMaxY})";

        return $"You said `{Message}` and could be heard {projectionBox}.";
    }

    public override void Setup(string decision)
    {
        var match = Regex.Match(decision, @$"{GetType().Name[..^6]}\([""'](.*?)[""'],\s*(\d+)\)");

        if (match.Success && int.TryParse(match.Groups[2].Value, out var projection))
        {
            Message = match.Groups[1].Value;
            Projection = projection;
        }
    }
}
