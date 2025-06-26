using AiPlayground.Api.Attributes;
using AiPlayground.Data.Repositories;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AiPlayground.Api.Actions;

public class LookAction(CharacterRepository characterRepository) : ActionBase, IAction
{
    private CharacterRepository _characterRepository = characterRepository;

    public override string Description => "Look at the specified coordinates in the grid.";

    [JsonPropertyName("x")]
    [ExampleValue(3)]
    public required int X { get; set; }

    [JsonPropertyName("y")]
    [ExampleValue(4)]
    public required int Y { get; set; }

    public async Task<string> Run(Guid characterId)
    {
        var characters = await _characterRepository.GetCharactersAsync();

        var sights = characters
            .SingleOrDefault(character => character.GridPosition.Item1 == X && character.GridPosition.Item2 == Y);

        if (sights is null)
        {
            return $"You see nothing of interest at ({X}, {Y}).";
        }

        return $"You see a {sights.Colour} character at ({X}, {Y}).";
    }

    public override void Setup(string decision)
    {
        var match = Regex.Match(decision, @$"{GetType().Name[..^6]}\((\d+), ?(\d+)\)");

        if (match.Success &&
            int.TryParse(match.Groups[1].Value, out var x) &&
            int.TryParse(match.Groups[2].Value, out var y))
        {
            X = x;
            Y = y;
        }
    }
}
