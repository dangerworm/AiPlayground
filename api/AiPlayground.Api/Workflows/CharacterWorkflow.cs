using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Workflows;

public class CharacterWorkflow(
    ILogger<CharacterWorkflow> logger,
    CharacterRepository characterRepository,
    ModelRepository modelRepository
) : WorkflowBase(logger)
{
    private readonly CharacterRepository _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
    private readonly ModelRepository _modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));

    public IEnumerable<string> GetAvailableModels()
    {
        return _modelRepository.GetModels();
    }

    public async Task<CharacterDto> CreateCharacterAsync(ConnectionDto connection, Tuple<int, int> gridPosition)
    {
        var character = await _characterRepository.CreateCharacter(connection, gridPosition);
        return character.AsDto();
    }
    public async Task<IEnumerable<CharacterDto>> GetCharactersAsync()
    {
        var characters = await _characterRepository.GetCharactersAsync();
        if (characters is null || !characters.Any())
        {
            return [];
        }

        var characterDtos = characters.Select(c => c.AsDto()).ToList();
        return characterDtos;
    }
    public async Task<CharacterDto> GetCharacterByIdAsync(Guid id)
    {
        var character = await _characterRepository.GetCharacterByIdAsync(id);
        return character.AsDto();
    }
    public async Task<CharacterDto> UpdateCharacterAgeByIdAsync(Guid id)
    {
        var character = await _characterRepository.UpdateCharacterAgeById(id);
        return character.AsDto();
    }
}
