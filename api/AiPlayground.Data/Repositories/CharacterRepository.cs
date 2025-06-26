using AiPlayground.Core.Constants;
using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Models.Conversations;
using AiPlayground.Data.Entities;

namespace AiPlayground.Data.Repositories;

public class CharacterRepository : JsonFileStore
{
    protected override string FileName => "Characters.json";

    public async Task<CharacterEntity> CreateCharacterAsync(int createdInIteration, string colour, ConnectionDto connectionDto, Tuple<int, int> gridPosition)
    {
        var connection = new ConnectionEntity
        {
            Id = connectionDto.Id,
            Endpoint = connectionDto.Endpoint,
            Model = connectionDto.Model,
            Host = connectionDto.Host,
            Port = connectionDto.Port,
            Temperature = connectionDto.Temperature
        };

        var character = new CharacterEntity
        {
            AgeInIterations = 0,
            CreatedInIteration = createdInIteration,
            Colour = colour,
            Connection = connection,
            GridPosition = gridPosition,
            Inputs = [],
            Responses = [],
            Questions = []
        };

        var characters = await LoadAsync<List<CharacterEntity>>() ?? [];
        var characterList = characters.ToList();
        characterList.Add(character);
        await SaveAsync(characterList);
        
        return character;
    }

    public async Task<IEnumerable<CharacterEntity>> GetCharactersAsync()
    {
        var characters = await LoadAsync<List<CharacterEntity>>();
        return characters ?? [];
    }

    public async Task<CharacterEntity> GetCharacterByIdAsync(Guid id)
    {
        var characters = await LoadAsync<List<CharacterEntity>>();
        return characters?.FirstOrDefault(c => c.Id == id)
               ?? throw new KeyNotFoundException($"Character with ID {id} not found.");
    }

    public async Task<CharacterEntity> AddIterationMessagesAsync(
        Guid characterId, 
        EnvironmentInputModel input, 
        CharacterResponseModel response
    )
    {
        var characters = await GetCharactersAsync();
        var character = characters?.FirstOrDefault(c => c.Id == characterId)
               ?? throw new KeyNotFoundException($"Character with ID {characterId} not found.");

        character.Inputs.Add(input);
        character.Responses.Add(response);
        character.AgeInIterations += 1;

        await SaveAsync(characters);

        return character;
    }

    public async Task<CharacterEntity> UpdatePositionByDeltaAsync(Guid characterId, int dx, int dy)
    {
        var characters = await GetCharactersAsync();
        var character = characters?.FirstOrDefault(c => c.Id == characterId)
               ?? throw new KeyNotFoundException($"Character with ID {characterId} not found.");

        var newX = Math.Max(0, Math.Min(PlaygroundConstants.DefaultGridSize - 1, character.GridPosition.Item1 + dx));
        var newY = Math.Max(0, Math.Min(PlaygroundConstants.DefaultGridSize - 1, character.GridPosition.Item2 + dy));

        var otherCharacters = characters.Where(c => c.Id != characterId).ToList();

        if (otherCharacters.Any(other => other.GridPosition.Item1 == newX &&
                                         other.GridPosition.Item2 == newY))
        {
            throw new InvalidOperationException($"Cannot move to ({newX}, {newY}) as another character is already there.");
        }

        character.GridPosition = new Tuple<int, int>(newX, newY);

        await SaveAsync(characters);

        return character;
    }
}
