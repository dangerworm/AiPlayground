using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AiPlayground.Api.Attributes;
using AiPlayground.Core.Constants;
using AiPlayground.Core.Enums;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Actions;

public class SpeakAction(CharacterRepository characterRepository) : ActionBase, IAction
{
    private readonly CharacterRepository _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));

    [IgnorePropertyDuringProcessing]
    public override ActionType Type => ActionType.ActionBased;

    [IgnorePropertyDuringProcessing]
    public override string Description => "Speak the string aloud, projecting to a radius of 'projection' across the grid.";

    [JsonPropertyName("message")]
    [ExampleValue("Hello, nice to meet you.")]
    public required string Message { get; set; }

    [JsonPropertyName("projection")]
    [ExampleValue(2)]
    public required int Projection { get; set; }

    public async Task<string> PreIteration(Guid characterId)
    {
        var character = await _characterRepository.GetCharacterByIdAsync(characterId);

        var projectionMinX = Math.Max(0, character.GridPosition.Item1 - Projection);
        var projectionMaxX = Math.Min(PlaygroundConstants.GridWidth - 1, character.GridPosition.Item1 + Projection);

        var projectionMinY = Math.Max(0, character.GridPosition.Item2 - Projection);
        var projectionMaxY = Math.Min(PlaygroundConstants.GridHeight - 1, character.GridPosition.Item2 + Projection);

        var projectionBox = $"from ({projectionMinX}, {projectionMinY}) to ({projectionMaxX}, {projectionMaxY})";

        return $"You said `{Message}` and could be heard {projectionBox}.";
    }

    public async Task PostIteration(Guid characterId)
    {
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
