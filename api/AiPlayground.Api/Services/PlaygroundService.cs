using System.Text.Json;
using System.Text.RegularExpressions;
using AiPlayground.Api.ViewModels;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.Constants;
using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Models.Conversations;

namespace AiPlayground.Api.Services;

public class PlaygroundService(
    ILogger<PlaygroundService> logger,
    IHttpClientFactory httpClientFactory,
    CharacterWorkflow characterWorkflow,
    PlaygroundWorkflow playgroundWorkflow,
    PromptWorkflow promptWorkflow
)
{
    private readonly ILogger<PlaygroundService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));
    private readonly PlaygroundWorkflow _playgroundWorkflow = playgroundWorkflow ?? throw new ArgumentNullException(nameof(playgroundWorkflow));
    private readonly PromptWorkflow _promptWorkflow = promptWorkflow ?? throw new ArgumentNullException(nameof(promptWorkflow));

    public async Task<PlaygroundViewModel> GetSetupAsync()
    {
        var characters = await _characterWorkflow.GetCharactersAsync();
        var playground = await _playgroundWorkflow.GetPlaygroundAsync();

        var model = new PlaygroundViewModel
        {
            AvailableModels = _characterWorkflow.GetAvailableModels().ToList(),
            Characters = characters.Select(c => new CharacterViewModel(c)).ToList(),
            CellSize = PlaygroundConstants.DefaultCellSizePixels,
            GridWidth = PlaygroundConstants.DefaultGridSize,
            GridHeight = PlaygroundConstants.DefaultGridSize,
        };

        return model;
    }

    public async Task<PlaygroundViewModel> Iterate()
    {
        var characters = await _characterWorkflow.GetCharactersAsync();
        var playground = await _playgroundWorkflow.GetPlaygroundAsync();
        var (characterEnvironmentInputs, characterMessages, characterQuestions) = await BuildCharacterData(characters);
        
        var llmCommunicationTasks = characters.Select(async character =>
        {
            try
            {
                var client = BuildClient(character.Connection);
                var ollamaInput = BuildOllamaInputModel(character, characterMessages[character.Id]);
                var response = await client.PostAsJsonAsync(character.Connection.Endpoint, ollamaInput);

                _logger.LogInformation("Generated output for character with ID '{Id}': {Response}", character.Id, response);

                var responseJson = await response.Content.ReadAsStringAsync();
                var characterResponse = ParseLlmResponse(responseJson, character.Id);

                character = await _characterWorkflow.AddIterationMessagesAsync(character.Id, characterEnvironmentInputs[character.Id], characterResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process character ID '{Id}'", character.Id);
            }
        });

        await Task.WhenAll(llmCommunicationTasks);

        await _playgroundWorkflow.UpdateIterationsAsync();

        characters = await _characterWorkflow.GetCharactersAsync();

        return new PlaygroundViewModel
        {
            AvailableModels = _characterWorkflow.GetAvailableModels().ToList(),
            Characters = characters.Select(c => new CharacterViewModel(c)),
            CellSize = PlaygroundConstants.DefaultCellSizePixels,
            GridWidth = PlaygroundConstants.DefaultGridSize,
            GridHeight = PlaygroundConstants.DefaultGridSize,
            Questions = characterQuestions.Any() ? characterQuestions : null
        };
    }

    private async Task<(
            IDictionary<Guid, EnvironmentInputModel>, 
            IDictionary<Guid, IList<MessageModel>>,
            IList<QuestionViewModel>
        )> BuildCharacterData(IEnumerable<CharacterDto> characters)
    {
        var sounds = new List<EnvironmentSoundProcessingModel>();
        var characterEnvironmentInputs = new Dictionary<Guid, EnvironmentInputModel>();
        var characterMessages = new Dictionary<Guid, IList<MessageModel>>();
        var characterQuestions = new List<QuestionViewModel>();

        // We have to do this one by one because things like Move() and Look() actions
        // can change the state of the character, and we need to ensure that each
        // character's state is fully prepared before we proceed to the next one.
        foreach (var character in characters)
        {
            var environmentInput = await _characterWorkflow.BuildEnvironmentInputAsync(character.Id);

            var question = environmentInput.ActionResults.SingleOrDefault(ar => ar.ActionName == "Ask");
            if (question is not null)
            {
                characterQuestions.Add(new QuestionViewModel
                {
                    CharacterId = character.Id,
                    Question = question.ActionResult.Split('`')[1],
                });
            }

            var speech = environmentInput.ActionResults.SingleOrDefault(ar => ar.ActionName == "Speak");
            if (speech is not null)
            {
                var match = Regex.Match(speech.ActionResult, @"could be heard from \((\-?\d+), (\-?\d+)\) to \((\-?\d+), (\-?\d+)\)");

                if (match.Success &&
                    int.TryParse(match.Groups[1].Value, out var minX) &&
                    int.TryParse(match.Groups[2].Value, out var minY) &&
                    int.TryParse(match.Groups[3].Value, out var maxX) &&
                    int.TryParse(match.Groups[4].Value, out var maxY))
                {
                    sounds.Add(new EnvironmentSoundProcessingModel
                    {
                        CharacterId = character.Id,
                        MinX = minX,
                        MinY = minY,
                        MaxX = maxX,
                        MaxY = maxY,
                        EnvironmentSoundModel = new EnvironmentSoundModel
                        {
                            Content = speech.ActionResult.Split('`')[1],
                            Source = $"The {character.Colour} character",
                            Type = "Speech"
                        }
                    });
                }
            }

            characterMessages[character.Id] = await BuildChatHistoryAsync(character.Id);
            characterEnvironmentInputs[character.Id] = environmentInput;
        }

        foreach (var character in characters)
        {
            characterEnvironmentInputs[character.Id].Sounds = sounds
                .Where(s => s.CharacterId != character.Id &&
                            s.MinX <= character.GridPosition.Item1 &&
                            s.MaxX >= character.GridPosition.Item1 &&
                            s.MinY <= character.GridPosition.Item2 &&
                            s.MaxY >= character.GridPosition.Item2)
                .Select(s => s.EnvironmentSoundModel);

            var inputJson = JsonSerializer.Serialize(characterEnvironmentInputs[character.Id]);
            characterMessages[character.Id].Add(new MessageModel
            {
                Role = "user",
                Content = inputJson
            });
        }

        return (characterEnvironmentInputs, characterMessages, characterQuestions);
    }

    private async Task<IList<MessageModel>> BuildChatHistoryAsync(Guid characterId)
    {
        var systemPrompt = _promptWorkflow.GetSystemPrompt();

        var systemMessage = new MessageModel { Role = "system", Content = systemPrompt };
        var chatHistory = await _characterWorkflow.BuildCorrelatedChatHistoryAsync(characterId);

        var messages = new List<MessageModel> { systemMessage };
        messages.AddRange(chatHistory);

        return messages;
    }

    private HttpClient BuildClient(ConnectionDto connection)
    {
        var client = _httpClientFactory.CreateClient(connection.Model);

        client.BaseAddress = new Uri($"http://{connection.Host}:{connection.Port}");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        _logger.LogInformation("Created HTTP client for model: {Model} at {Endpoint}", connection.Model, client.BaseAddress);

        return client;
    }

    private OllamaInputModel BuildOllamaInputModel(CharacterDto character, IEnumerable<MessageModel> messages)
    {
        return new OllamaInputModel
        {
            Model = character.Connection.Model,
            Messages = messages,
            Temperature = character.Connection.Temperature,
        };
    }

    private CharacterResponseModel ParseLlmResponse(string responseJson, Guid characterId)
    {
        MessageModel? message;

        try
        {
            message = JsonSerializer.Deserialize<OllamaResponseModel>(responseJson)?.Message;
            if (message is null)
            {
                _logger.LogError("Deserialized message is null for character ID '{Id}': {Response}", characterId, responseJson);
                throw new InvalidOperationException($"Deserialized message is null for character ID '{characterId}'.");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for character ID '{Id}': {Response}", characterId, responseJson);
            throw new InvalidOperationException($"Failed to deserialize response for character ID '{characterId}'.", ex);
        }

        try
        {
            // The LLMs don't always respond with valid JSON, so we need to handle that gracefully.
            // Most of the time it's due to missing commas or other formatting issues, so I'll set
            // up some basic error handling here.

            var json = message.Content.Trim();

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
            _logger.LogError(ex, "Failed to deserialize response for character ID '{Id}': {Response}", characterId, responseJson);
            throw new InvalidOperationException($"Failed to deserialize response for character ID '{characterId}'.", ex);
        }
    }

}
