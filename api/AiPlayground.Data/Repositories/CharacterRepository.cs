using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Data.Entities;

namespace AiPlayground.Data.Repositories;

public class CharacterRepository : JsonFileStore
{
    protected override string FilePath => "Characters.json";

    public async Task<CharacterEntity> CreateCharacter(ConnectionDto connectionDto, Tuple<int, int> gridPosition)
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
            AgeInEnvironmentIterations = 0,
            Connection = connection,
            GridPosition = gridPosition
        };

        var characters = await LoadAsync<IEnumerable<CharacterEntity>>() ?? [];
        var characterList = characters.ToList();
        characterList.Add(character);
        await SaveAsync(characterList);
        
        return character;
    }

    public async Task<IEnumerable<CharacterEntity>> GetCharactersAsync()
    {
        var characters = await LoadAsync<IEnumerable<CharacterEntity>>();
        return characters ?? [];
    }

    public async Task<CharacterEntity> GetCharacterByIdAsync(Guid id)
    {
        var characters = await LoadAsync<IEnumerable<CharacterEntity>>();
        return characters?.FirstOrDefault(c => c.Id == id)
               ?? throw new KeyNotFoundException($"Character with ID {id} not found.");
    }

    public async Task<CharacterEntity> UpdateCharacterAgeById(Guid id)
    {
        var characters = await GetCharactersAsync();
        var character = characters?.FirstOrDefault(c => c.Id == id)
               ?? throw new KeyNotFoundException($"Character with ID {id} not found.");

        character.AgeInEnvironmentIterations += 1;

        await SaveAsync(characters);
        
        return character;
    }
}
