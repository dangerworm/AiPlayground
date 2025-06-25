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

    public async Task<IList<CharacterEntity>> GetCharactersAsync()
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

        var newX = Math.Min(0, Math.Max(PlaygroundConstants.DefaultGridSize, character.GridPosition.Item1 + dx));
        var newY = Math.Min(0, Math.Max(PlaygroundConstants.DefaultGridSize, character.GridPosition.Item2 + dy));

        character.GridPosition = new Tuple<int, int>(newX, newY);

        await SaveAsync(characters);

        return character;
    }
}
