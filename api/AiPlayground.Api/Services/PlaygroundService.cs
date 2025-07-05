using System.Text.Json;
using System.Text.RegularExpressions;
using AiPlayground.Api.ViewModels;
using AiPlayground.Api.ViewModels.Inputs;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.Constants;
using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Enums;
using AiPlayground.Core.Models.Conversations;
using OpenAI.Chat;

namespace AiPlayground.Api.Services;

public class PlaygroundService(
    ILogger<PlaygroundService> logger,
    ChatClient chatClient,
    CharacterWorkflow characterWorkflow,
    PlaygroundWorkflow playgroundWorkflow,
    PromptWorkflow promptWorkflow
)
{
    private readonly ILogger<PlaygroundService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ChatClient _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));
    private readonly PlaygroundWorkflow _playgroundWorkflow = playgroundWorkflow ?? throw new ArgumentNullException(nameof(playgroundWorkflow));
    private readonly PromptWorkflow _promptWorkflow = promptWorkflow ?? throw new ArgumentNullException(nameof(promptWorkflow));

    private readonly ChatCompletionOptions _chatClientOptions = new()
    {
        MaxOutputTokenCount = AzureOpenAiConstants.MaxCompletionTokens,
        Temperature = AzureOpenAiConstants.Temperature,
        TopP = AzureOpenAiConstants.TopP,
        FrequencyPenalty = AzureOpenAiConstants.FrequencyPenalty,
        PresencePenalty = AzureOpenAiConstants.PresencePenalty,
    };

    public async Task<PlaygroundViewModel> GetSetupAsync()
    {
        var characters = await _characterWorkflow.GetCharactersAsync();
        var playground = await _playgroundWorkflow.GetPlaygroundAsync();

        var model = new PlaygroundViewModel
        {
            AvailableModels = [.. _characterWorkflow.GetAvailableModels()],
            Characters = [.. characters.Select(c => new CharacterViewModel(c))],
            GridWidth = PlaygroundConstants.GridWidth,
            GridHeight = PlaygroundConstants.GridHeight,
            Items = [.. playground.Items.Select(i => new ItemViewModel(i))],
            Iteration = playground.Iterations
        };

        return model;
    }

    public async Task<PlaygroundViewModel> IterateAsync(InteractionInputViewModel interactionInputViewModel)
    {
        var isValid = IsInputModelValid(interactionInputViewModel);
        if (!isValid)
        {
            return await GetSetupAsync();
        }

        if (interactionInputViewModel.QuestionAnswers is not null)
        {
            var questionAnswers = interactionInputViewModel.QuestionAnswers.Select(qa => qa.FromViewModel());
            await _characterWorkflow.AddQuestionAnswers(questionAnswers);
        }

        var characters = await _characterWorkflow.GetCharactersAsync();
        var (characterEnvironmentInputs, characterMessages) = await BuildCharacterDataAsync(characters);

        var llmCommunicationTasks = characters.Select(async character =>
        {
            try
            {
                var response = await _chatClient.CompleteChatAsync(characterMessages[character.Id], _chatClientOptions);

                var responseJson = response.Value.Content[0].Text;
                var characterResponse = ParseLlmResponse(responseJson, character.Id);

                character = await _characterWorkflow.AddIterationMessagesAsync(character.Id, characterEnvironmentInputs[character.Id], characterResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process character ID '{Id}'", character.Id);
            }
        });

        await Task.WhenAll(llmCommunicationTasks);

        await UpdateCharacterDataAsync(characters);

        characters = await _characterWorkflow.GetCharactersAsync();
        var playground = await _playgroundWorkflow.UpdateIterationsAsync();

        return new PlaygroundViewModel
        {
            AvailableModels = [.. _characterWorkflow.GetAvailableModels()],
            Characters = characters.Select(c => new CharacterViewModel(c)),
            GridWidth = PlaygroundConstants.GridWidth,
            GridHeight = PlaygroundConstants.GridHeight,
            Iteration = playground.Iterations,
            Items = playground.Items?.Select(i => new ItemViewModel(i))
        };
    }

    public async Task ResetPlaygroundAsync()
    {
        await _playgroundWorkflow.ResetPlaygroundAsync();
        _logger.LogInformation("Playground state reset");
    }

    private bool IsInputModelValid(InteractionInputViewModel inputViewModel)
    {
        if (inputViewModel.QuestionAnswers is null)
        {
            return true;
        }

        if (inputViewModel.QuestionAnswers.Any(qa => qa.Answer is null))
        {
            _logger.LogWarning("One or more answers missing from input view model.");
            return false;
        }

        return true;
    }

    private async Task<(
            IDictionary<Guid, EnvironmentInputModel>,
            IDictionary<Guid, List<ChatMessage>>
        )> BuildCharacterDataAsync(IEnumerable<CharacterDto> characters)
    {
        var characterEnvironmentInputs = new Dictionary<Guid, EnvironmentInputModel>();
        var characterMessages = new Dictionary<Guid, List<ChatMessage>>();
        var characterSightsSeen = new List<string>();
        var characterSoundsHeard = new List<EnvironmentSoundProcessingModel>();

        // We have to do this bit by bit and pull out relevant information because things
        // like Move() and Look() are inter-dependent, and Speak() makes sounds which will
        // need to be communicated to *all* affected characters regardless of when the
        // speaking occurred.
        foreach (var character in characters)
        {
            var actionResults = await _characterWorkflow.ProcessActionsAsync(character.Id, IterationStage.PreIteration);
            var environmentInput = await _characterWorkflow.BuildEnvironmentInputAsync(character.Id, actionResults);

            var sights = environmentInput.ActionResults.Where(ar => ar.ActionName == "Look");
            if (sights.Any())
            {
                characterSightsSeen.AddRange(sights.Select(sight => sight.ActionResult));
            }

            var speech = environmentInput.ActionResults.Where(ar => ar.ActionName == "Speak");
            foreach (var phrase in speech)
            {
                var match = Regex.Match(phrase.ActionResult, @"could be heard from \((\-?\d+), (\-?\d+)\) to \((\-?\d+), (\-?\d+)\)");

                if (match.Success &&
                    int.TryParse(match.Groups[1].Value, out var minX) &&
                    int.TryParse(match.Groups[2].Value, out var minY) &&
                    int.TryParse(match.Groups[3].Value, out var maxX) &&
                    int.TryParse(match.Groups[4].Value, out var maxY))
                {
                    characterSoundsHeard.Add(new EnvironmentSoundProcessingModel
                    {
                        CharacterId = character.Id,
                        MinX = minX,
                        MinY = minY,
                        MaxX = maxX,
                        MaxY = maxY,
                        EnvironmentSoundModel = new EnvironmentSoundModel
                        {
                            Content = phrase.ActionResult.Split('`')[1],
                            Source = $"The {character.Colour} character",
                            Type = "Speech"
                        }
                    });
                }
            }

            characterEnvironmentInputs[character.Id] = environmentInput;
            characterMessages[character.Id] = await BuildChatHistoryAsync(character.Id);
        }

        foreach (var character in characters)
        {
            characterEnvironmentInputs[character.Id].Sounds = characterSoundsHeard
                .Where(s => s.CharacterId != character.Id &&
                            s.MinX <= character.GridPosition.Item1 &&
                            s.MaxX >= character.GridPosition.Item1 &&
                            s.MinY <= character.GridPosition.Item2 &&
                            s.MaxY >= character.GridPosition.Item2)
                .Select(s => s.EnvironmentSoundModel);

            var inputJson = JsonSerializer.Serialize(characterEnvironmentInputs[character.Id]);
            characterMessages[character.Id].Add(new UserChatMessage(inputJson));
        }

        return (characterEnvironmentInputs, characterMessages);
    }

    private async Task<List<ChatMessage>> BuildChatHistoryAsync(Guid characterId)
    {
        var systemPrompt = _promptWorkflow.GetSystemPrompt();

        var systemMessage = new SystemChatMessage(systemPrompt);
        var chatHistory = await _characterWorkflow.BuildCorrelatedChatHistoryAsync(characterId);

        var messages = new List<ChatMessage> { systemMessage };
        messages.AddRange(chatHistory);

        return messages;
    }

    private CharacterResponseModel ParseLlmResponse(string llmResponse, Guid characterId)
    {
        try
        {
            // The LLMs don't always respond with valid JSON, so we need to handle that gracefully.
            // Most of the time it's due to missing commas or other formatting issues, so I'll set
            // up some basic error handling here.

            var json = llmResponse.Trim();

            // Add missing commas between object properties: "value" "key":
            json = Regex.Replace(json, @"\""\s+\""(?!\s*[:,}\]])", "\", \"");

            // Optional: remove extra spaces around colons (safe-ish)
            json = Regex.Replace(json, @"\s*:\s*", ":");

            // Try to extract the last valid-looking JSON object
            var match = Regex.Matches(json, @"\{(?:[^{}]|(?<open>\{)|(?<-open>\}))*\}(?(open)(?!))");
            if (match.Count == 0)
            {
                _logger.LogError("No valid JSON object found in content: {Content}", json);
                throw new InvalidOperationException("No valid JSON object found in LLM response.");
            }

            var lastJsonObject = match[^1].Value;

            var response = JsonSerializer.Deserialize<CharacterResponseModel>(lastJsonObject);

            if (response is null)
            {
                _logger.LogError("Deserialized response is null for character ID '{Id}': {Json}", characterId, lastJsonObject);
                throw new InvalidOperationException($"Deserialized response is null for character ID '{characterId}'.");
            }

            return response;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for character ID '{Id}': {Response}", characterId, llmResponse);
            throw new InvalidOperationException($"Failed to deserialize response for character ID '{characterId}'.", ex);
        }
    }

    private async Task UpdateCharacterDataAsync(IEnumerable<CharacterDto> characters)
    {
        foreach (var character in characters)
        {
            await _characterWorkflow.ProcessActionsAsync(character.Id, IterationStage.PostIteration);
        }
    }
}
