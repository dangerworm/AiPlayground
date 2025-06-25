using AiPlayground.Api.Actions;
using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Models.Conversations;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Workflows;

public class CharacterWorkflow(
    ILogger<CharacterWorkflow> logger,
    ActionProcessor actionProcessor,
    CharacterRepository characterRepository,
    ModelRepository modelRepository,
    PlaygroundRepository playgroundRepository
) : WorkflowBase(logger)
{
    private readonly ActionProcessor _actionProcessor = actionProcessor ?? throw new ArgumentNullException(nameof(actionProcessor));
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

    public async Task<CharacterDto> AddIterationMessagesAsync(
        Guid characterId, 
        EnvironmentInputModel input, 
        CharacterResponseModel response
    )
    {
        var correlationId = Guid.NewGuid();

        input.CorrelationId = correlationId;
        response.CorrelationId = correlationId;

        var character = await _characterRepository.AddIterationMessagesAsync(characterId, input, response);
        return character.AsDto();
    }

    public async Task<EnvironmentInputModel> BuildEnvironmentInputAsync(Guid characterId)
    {
        var character = await _characterRepository.GetCharacterByIdAsync(characterId);
        var actionResults = new List<EnvironmentActionResultModel>();
        _actionProcessor.ProcessActions(character.AsDto());
        var environment = await _playgroundRepository.GetPlaygroundAsync();
        var sounds = new List<EnvironmentSoundModel>();

        var input = new EnvironmentInputModel
        {
            ActionResults = actionResults,
            Age = character.AgeInIterations,
            GridPosition = $"[{character.GridPosition.Item1},{character.GridPosition.Item2}]",
            Environment = await DescribeCharacterEnvironmentAsync(characterId),
            Iteration = environment.Iterations,
            Sounds = sounds,
            Time = environment.Iterations
        };

        return input;
    }

    private async Task<string> DescribeCharacterEnvironmentAsync(Guid characterId)
    {
        var characters = await _characterRepository.GetCharactersAsync();
        if (characters is null || !characters.Any())
        {
            return "No characters found in the environment.";
        }

        if (!characters.Any(c => c.Id != characterId))
        {
            return $"No other characters found in the environment.";
        }

        var descriptions = characters.Select(c => $"Character {c.Id} is at position {c.GridPosition}.").ToList();
        return string.Join(Environment.NewLine, descriptions);
    }
}
