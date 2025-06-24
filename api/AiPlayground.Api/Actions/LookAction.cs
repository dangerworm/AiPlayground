using AiPlayground.Api.Attributes;
using AiPlayground.Data.Repositories;
using System.Text.Json.Serialization;

namespace AiPlayground.Api.Actions;

public class LookAction : ActionBase, IAction
{
    private CharacterRepository _characterRepository;
    
    public override string Description => "Look at the specified coordinates in the grid.";

    public LookAction(CharacterRepository characterRepository)
    {
        _characterRepository = characterRepository;
    }

    [JsonPropertyName("x")]
    [ExampleValue(3)]
    public required int X { get; set; }

    [JsonPropertyName("y")]
    [ExampleValue(4)]
    public required int Y { get; set; }

    public async Task<string?> Run(Guid characterId)
    {
        var characters = await _characterRepository.GetCharactersAsync();

        var sights = characters
            .Where(character => character.GridPosition.Item1 == X && character.GridPosition.Item2 == Y);

        return sights is null || !sights.Any() 
            ? $"You see nothing of interest at ({X}, {Y})." 
            : $"You see another character at ({X}, {Y}).";
    }
}
