using System.Text.Json;
using System.Text.Json.Serialization;
using AiPlayground.Api.Models.Conversations;
using AiPlayground.Api.Models.Interactions;
using AiPlayground.Api.ViewModels;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Api.Services;

public class InteractionService(
    ILogger<InteractionService> logger,
    IHttpClientFactory httpClientFactory,
    CharacterWorkflow characterWorkflow,
    CharacterEnvironmentWorkflow characterEnvironmentWorkflow,
    PromptWorkflow promptWorkflow
)
{
    private readonly ILogger<InteractionService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));
    private readonly CharacterEnvironmentWorkflow _characterEnvironmentWorkflow = characterEnvironmentWorkflow ?? throw new ArgumentNullException(nameof(characterEnvironmentWorkflow));
    private readonly PromptWorkflow _promptWorkflow = promptWorkflow ?? throw new ArgumentNullException(nameof(promptWorkflow));

    public async Task<CharacterViewModel> ProcessInteraction(InteractInputModel model)
    {
        var character = await _characterWorkflow.GetCharacterByIdAsync(model.CharacterId);
        var client = BuildClient(character.Connection);
        var chatModel = await BuildChatModelAsync(character);

        var response = await client.PostAsJsonAsync(character.Connection.Endpoint, chatModel);

        _logger.LogInformation("Generated output for character with ID '{Id}': {Response}", character.Id, response);

        var responseJson = await response.Content.ReadAsStringAsync();
        var characterResponse = ParseLlmResponse(responseJson, character);

        character = await _characterWorkflow.AddMessageAsync(character.Id, characterResponse);

        return new CharacterViewModel(character);
    }

    public async Task<OllamaInputModel> BuildChatModelAsync(CharacterDto character)
    {
        var systemPrompt = _promptWorkflow.GetSystemPrompt();

        var messages = new List<MessageModel>
        {
            new MessageModel { Role = "system", Content = systemPrompt }
        };

        foreach(var response in character.Responses)
        {
            messages.Add(new MessageModel { Role = "assistant", Content = JsonSerializer.Serialize(response) });
        }

        var output = await _characterEnvironmentWorkflow.BuildEnvironmentOutputAsync(character.Id);
        messages.Add(new MessageModel { Role = "user", Content = output });

        return new OllamaInputModel
        {
            Messages = messages,
            Model = character.Connection.Model,
            Temperature = character.Connection.Temperature,
        };
    }

    private HttpClient BuildClient(ConnectionDto connection)
    {
        var client = _httpClientFactory.CreateClient(connection.Model);

        client.BaseAddress = new Uri($"http://{connection.Host}:{connection.Port}");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        _logger.LogInformation("Created HTTP client for model: {Model} at {Endpoint}", connection.Model, client.BaseAddress);

        return client;
    }

    private CharacterResponseModel ParseLlmResponse(string responseJson, CharacterDto character)
    {
        try
        {
            var message = JsonSerializer.Deserialize<OllamaResponseModel>(responseJson)?.Message;
            if (message is null)
            {
                _logger.LogError("Deserialized message is null for character ID '{Id}': {Response}", character.Id, responseJson);
                throw new InvalidOperationException($"Deserialized message is null for character ID '{character.Id}'.");
            }

            var response = JsonSerializer.Deserialize<CharacterResponseModel>(message.Content.Trim());
            if (response is null)
            {
                _logger.LogError(responseJson, "Deserialized response is null for character ID '{Id}': {Response}", character.Id, responseJson);
                throw new InvalidOperationException($"Deserialized response is null for character ID '{character.Id}'.");
            }

            return response;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for character ID '{Id}': {Response}", character.Id, responseJson);
            throw new InvalidOperationException($"Failed to deserialize response for character ID '{character.Id}'.", ex);
        }
    }
}
