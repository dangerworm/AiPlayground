using System.Globalization;
using System.Text.Json;
using AiPlayground.Api.Actions;
using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Enums;
using AiPlayground.Core.Models;
using AiPlayground.Core.Models.Conversations;
using AiPlayground.Core.Models.Interactions;
using AiPlayground.Data.Repositories;
using OpenAI.Chat;

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

    private readonly List<ActionType> _actionProcessingOrder =
    [
        ActionType.EnvironmentBased,
        ActionType.CharacterBased,
        ActionType.ActionBased
    ];

    public IEnumerable<string> GetAvailableModels()
    {
        return _modelRepository.GetModels();
    }

    public async Task<CharacterDto> CreateCharacterAsync(string colour, Tuple<int, int> gridPosition)
    {
        var playground = await _playgroundRepository.GetPlaygroundAsync();
        var character = await _characterRepository.CreateCharacterAsync(playground.Iterations, colour, gridPosition);
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
        if (character is null)
        {
            Logger.LogError("Character with ID '{Id}' not found.", id);
            throw new ArgumentException($"Character with ID '{id}' not found.");
        }

        return character.AsDto();
    }
    
    public Task AddQuestionAnswers(IEnumerable<QuestionAnswerModel> questionAnswers)
    {
        return _characterRepository.AddQuestionAnswers(questionAnswers);
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

    public async Task<List<EnvironmentActionResultModel>> ProcessActionsAsync(Guid characterId, IterationStage processingPoint)
    {
        var character = await _characterRepository.GetCharacterByIdAsync(characterId);

        var actionResults = new List<EnvironmentActionResultModel>();
        foreach (var actionType in _actionProcessingOrder)
        {
            var results = await _actionProcessor.ProcessActions(character.AsDto(), processingPoint, actionType);
            if (results.Any())
            {
                Logger.LogInformation("Processed {ActionType} actions for character {CharacterId}.", actionType, characterId);
                actionResults.AddRange(results);
            }
        }

        return actionResults;
    }

    public async Task<EnvironmentInputModel> BuildEnvironmentInputAsync(Guid characterId, List<EnvironmentActionResultModel> actionResults)
    {
        var character = await _characterRepository.GetCharacterByIdAsync(characterId);
        var playground = await _playgroundRepository.GetPlaygroundAsync();

        var input = new EnvironmentInputModel
        {
            ActionResults = actionResults,
            Age = character.AgeInIterations,
            Name = character.Name,
            GridPosition = $"[{character.GridPosition.Item1},{character.GridPosition.Item2}]",
            Environment = await DescribeCharacterEnvironmentAsync(characterId),
            Iteration = playground.Iterations,
            Sounds = [],
            DateTimeIso8601 = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
        };

        return input;
    }

    public async Task<IEnumerable<ChatMessage>> BuildCorrelatedChatHistoryAsync(Guid characterId)
    {
        var character = await GetCharacterByIdAsync(characterId);

        var inputs = character.Inputs as IEnumerable<ICorrelated>;
        var responses = character.Responses as IEnumerable<ICorrelated>;

        var messageGroups = inputs
            .Union(responses)
            .GroupBy(message => message.CorrelationId)
            .Reverse()
            .Take(10)
            .Reverse()
            .ToList();

        var messages = new List<ChatMessage>();
        foreach (var group in messageGroups)
        {
            if (group.FirstOrDefault(m => m is EnvironmentInputModel) is not EnvironmentInputModel input ||
                group.FirstOrDefault(m => m is CharacterResponseModel) is not CharacterResponseModel response)
            {
                var error = $"{nameof(EnvironmentInputModel)} or {nameof(CharacterResponseModel)} is null";
                Logger.LogError("{Error} for correlation ID {CorrelationID}", error, group.Key);
                throw new InvalidOperationException($"{error} for correlation ID '{group.Key}'.");
            }

            input.CorrelationId = null;
            response.CorrelationId = null;

            messages.Add(new UserChatMessage(JsonSerializer.Serialize(input)));
            messages.Add(new AssistantChatMessage(JsonSerializer.Serialize(response)));
        }

        return messages;
    }

    private async Task<string> DescribeCharacterEnvironmentAsync(Guid characterId)
    {
        var characters = await _characterRepository.GetCharactersAsync();
        if (characters is null || !characters.Any())
        {
            return "No characters found in the environment.";
        }

        var otherCharacters = characters.Where(c => c.Id != characterId);
        if (!otherCharacters.Any())
        {
            return $"No other characters found in the environment.";
        }

        var descriptions = otherCharacters.Select(c => $"A {c.Colour} character is at position {c.GridPosition}.").ToList();
        return string.Join(Environment.NewLine, descriptions);
    }
}
