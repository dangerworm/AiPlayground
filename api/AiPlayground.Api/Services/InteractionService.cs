using System.Text.Json;
using AiPlayground.Api.ViewModels;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Models;
using AiPlayground.Core.Models.Conversations;
using AiPlayground.Core.Models.Interactions;

namespace AiPlayground.Api.Services;

public class InteractionService(
    ILogger<InteractionService> logger,
    IHttpClientFactory httpClientFactory,
    CharacterWorkflow characterWorkflow,
    PromptWorkflow promptWorkflow
)
{
    private readonly ILogger<InteractionService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));
    private readonly PromptWorkflow _promptWorkflow = promptWorkflow ?? throw new ArgumentNullException(nameof(promptWorkflow));

    public async Task<CharacterViewModel> ProcessInteraction(InteractInputModel model)
    {
        var character = await _characterWorkflow.GetCharacterByIdAsync(model.CharacterId);
        var messages = BuildChatHistoryAsync(character);

        var environmentInput = await _characterWorkflow.BuildEnvironmentInputAsync(character.Id);
        var inputJson = JsonSerializer.Serialize(environmentInput);
        messages.Add(new MessageModel { Role = "user", Content = inputJson });

        var client = BuildClient(character.Connection);
        var ollamaInput = BuildOllamaInputModel(character, messages);
        var response = await client.PostAsJsonAsync(character.Connection.Endpoint, ollamaInput);

        _logger.LogInformation("Generated output for character with ID '{Id}': {Response}", character.Id, response);

        var responseJson = await response.Content.ReadAsStringAsync();
        var characterResponse = ParseLlmResponse(responseJson, character.Id);

        character = await _characterWorkflow.AddIterationMessagesAsync(character.Id, environmentInput, characterResponse);

        return new CharacterViewModel(character);
    }

    private IList<MessageModel> BuildChatHistoryAsync(CharacterDto character)
    {
        var systemPrompt = _promptWorkflow.GetSystemPrompt();

        var messages = new List<MessageModel>
        {
            new MessageModel { Role = "system", Content = systemPrompt }
        };

        BuildCorrelatedConversationMessages(character, messages);

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

    private void BuildCorrelatedConversationMessages(CharacterDto character, IList<MessageModel> messages)
    {
        var inputs = character.Inputs as IEnumerable<ICorrelated>;
        var responses = character.Responses as IEnumerable<ICorrelated>;

        var messageGroups = inputs
            .Union(responses)
            .GroupBy(message => message.CorrelationId);

        foreach (var group in messageGroups)
        {
            if (group.FirstOrDefault(m => m is EnvironmentInputModel) is not EnvironmentInputModel input ||
                group.FirstOrDefault(m => m is CharacterResponseModel) is not CharacterResponseModel response)
            {
                var error = $"{nameof(EnvironmentInputModel)} or {nameof(CharacterResponseModel)} is null";
                _logger.LogError("{Error} for correlation ID {CorrelationID}", error, group.Key);
                throw new InvalidOperationException($"{error} for correlation ID '{group.Key}'.");
            }

            input.CorrelationId = null;
            response.CorrelationId = null;

            messages.Add(new MessageModel { Role = "user", Content = JsonSerializer.Serialize(input) });
            messages.Add(new MessageModel { Role = "assistant", Content = JsonSerializer.Serialize(response) });
        }
    }

    private OllamaInputModel BuildOllamaInputModel(CharacterDto character, IList<MessageModel> messages)
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
            var response = JsonSerializer.Deserialize<CharacterResponseModel>(message.Content.Trim());
            if (response is null)
            {
                _logger.LogError(responseJson, "Deserialized response is null for character ID '{Id}': {Response}", characterId, responseJson);
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
