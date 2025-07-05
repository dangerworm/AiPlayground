using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AiPlayground.Api.Attributes;
using AiPlayground.Core.Constants;
using AiPlayground.Core.Enums;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Actions;

public class MoveAction(CharacterRepository characterRepository) : ActionBase, IAction
{
    private readonly CharacterRepository _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));

    [IgnorePropertyDuringProcessing]
    public override ActionType Type => ActionType.ActionBased;

    [IgnorePropertyDuringProcessing]
    public override string Description => "Move in the direction specified by dx and dy where -1 <= dx <= 1 and -1 <= dy <= 1.";

    [JsonPropertyName("dx")]
    [ExampleValue(3)]
    public required int Dx { get; set; }

    [JsonPropertyName("dy")]
    [ExampleValue(4)]
    public required int Dy { get; set; }

    public async Task<string> PreIteration(Guid characterId)
    {
        Dx = Math.Max(-1, Math.Min(1, Dx));
        Dy = Math.Max(-1, Math.Min(1, Dy));

        var character = await _characterRepository.GetCharacterByIdAsync(characterId);
        var newX = character.GridPosition.Item1 + Dx;
        var newY = character.GridPosition.Item2 + Dy;

        var edgeResults = new List<string>() { "You are at the edge of the grid." };
        if (newX < 0 || newX > PlaygroundConstants.GridWidth)
        {
            edgeResults.Add($"You cannot move {(Dx < 0 ? Dx : $"+{Dx}")} in the X direction.");
        }

        if (newY < 0 || newY > PlaygroundConstants.GridHeight)
        {
            edgeResults.Add($"You cannot move {(Dy < 0 ? Dy : $"+{Dy}")} in the Y direction.");
        }

        if (edgeResults.Count > 1)
        {
            return string.Join(" ", edgeResults);
        }

        try
        {
            character = await _characterRepository.UpdatePositionByDeltaAsync(characterId, Dx, Dy);
            return $"You moved to ({character.GridPosition.Item1}, {character.GridPosition.Item2}).";
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }
    }

    public async Task PostIteration(Guid characterId)
    {
    }

    public override void Setup(string decision)
    {
        var match = Regex.Match(decision, @$"{GetType().Name[..^6]}\((-?\d+), ?(-?\d+)\)");

        if (match.Success &&
            int.TryParse(match.Groups[1].Value, out var dx) &&
            int.TryParse(match.Groups[2].Value, out var dy))
        {
            Dx = dx;
            Dy = dy;
        }
    }
}
