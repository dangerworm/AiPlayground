using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Data.Entities;

namespace AiPlayground.Data.Repositories;

public class CharacterRepository : JsonFileStore
{
    protected override string FilePath => "Characters.json";

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

    public async Task<CharacterEntity> AddMessageAsync(
        Guid characterId, 
        IList<string> decisions,
        IList<string> desires,
        string emotion,
        string thoughts
    )
    {
        var characters = await GetCharactersAsync();
        var character = characters?.FirstOrDefault(c => c.Id == characterId)
               ?? throw new KeyNotFoundException($"Character with ID {characterId} not found.");

        var response = new CharacterResponseEntity
        {
            Decisions = decisions,
            Desires = desires,
            Emotion = emotion,
            Thoughts = thoughts
        };

        character.AgeInIterations += 1;
        character.Responses.Add(response);

        await SaveAsync(characters);

        return character;
    }
}
