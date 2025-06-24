using AiPlayground.Api.Models.Conversations;
using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Workflows;

public class CharacterWorkflow(
    ILogger<CharacterWorkflow> logger,
    CharacterRepository characterRepository,
    ModelRepository modelRepository,
    PlaygroundRepository playgroundRepository
) : WorkflowBase(logger)
{
    private readonly CharacterRepository _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
    private readonly ModelRepository _modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
    private readonly PlaygroundRepository _playgroundRepository = playgroundRepository ?? throw new ArgumentNullException(nameof(playgroundRepository));

    public IList<string> GetAvailableModels()
    {
        return _modelRepository.GetModels();
    }

    public async Task<CharacterDto> CreateCharacterAsync(string colour, ConnectionDto connection, Tuple<int, int> gridPosition)
    {
        var playground = await _playgroundRepository.GetPlaygroundAsync();
        var character = await _characterRepository.CreateCharacterAsync(playground.Iterations, colour, connection, gridPosition);
        return character.AsDto();
    }

    public async Task<IList<CharacterDto>> GetCharactersAsync()
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
        if (character is null)
        {
            Logger.LogError("Character with ID '{Id}' not found.", id);
            throw new ArgumentException($"Character with ID '{id}' not found.");
        }

        return character.AsDto();
    }

    public async Task<CharacterDto> AddMessageAsync(Guid characterId, CharacterResponseModel response)
    {
        var character = await _characterRepository.AddMessageAsync(
            characterId, 
            response.Decisions,
            response.Desires,
            response.Emotion,
            response.Thoughts
        );

        return character.AsDto();
    }
}
