using System.Text.Json;
using System.Text.Json.Serialization;
using AiPlayground.Api.Models.Conversations;
using AiPlayground.Api.Models.Interactions;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Api.Services;

public class InteractionService(
    IHttpClientFactory httpClientFactory,
    ILogger<InteractionService> logger,
    CharacterWorkflow characterWorkflow,
    CharacterEnvironmentWorkflow characterEnvironmentWorkflow,
    PlaygroundWorkflow playgroundWorkflow,
    PromptWorkflow promptWorkflow
)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly ILogger<InteractionService> _logger = logger;
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));
    private readonly CharacterEnvironmentWorkflow _characterEnvironmentWorkflow = characterEnvironmentWorkflow ?? throw new ArgumentNullException(nameof(characterEnvironmentWorkflow));
    private readonly PlaygroundWorkflow _playgroundWorkflow = playgroundWorkflow ?? throw new ArgumentNullException(nameof(playgroundWorkflow));
    private readonly PromptWorkflow _promptWorkflow = promptWorkflow ?? throw new ArgumentNullException(nameof(promptWorkflow));

    public async Task<MessageModel> ContactLlmAsync(InteractInputModel characterInput)
    {
        var character = await _characterWorkflow.GetCharacterByIdAsync(characterInput.CharacterId);
        if (character is null)
        {
            _logger.LogError("Character with ID '{Id}' not found.", characterInput.CharacterId);
            throw new ArgumentException($"Character with ID '{characterInput.CharacterId}' not found.");
        }

        var chatModel = await BuildChatModelAsync(character);
        var client = BuildClient(character.Connection);

        var response = await client.PostAsJsonAsync(character.Connection.Endpoint, chatModel);

        _logger.LogInformation("Generated output for character with ID '{Id}': {Response}", character.Id, response);

        var responseJson = await response.Content.ReadAsStringAsync();
        return ParseOllamaResponse(responseJson, character);
    }

    public async Task<OllamaInputModel> BuildChatModelAsync(CharacterDto character)
    {
        var systemPrompt = _promptWorkflow.GetFirstMessage();
        var output = await _characterEnvironmentWorkflow.BuildEnvironmentOutputAsync(character.Id);

        var messages = new List<MessageModel>
        {
            { new MessageModel { Role = "system", Content = systemPrompt } },
            { new MessageModel { Role = "user", Content = output } }
        };

        /* Will need previous content at some point
         * 
    [JsonPropertyName("decisions")]
    public string[] Decisions { get; set; } = [];

    [JsonPropertyName("desires")]
    public string[] Desires { get; set; } = [];

    [JsonPropertyName("emotion")]
    public string Emotion { get; set; } = string.Empty;

    [JsonPropertyName("thoughts")]
    public string Thoughts { get; set; } = string.Empty;
        */

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

    private MessageModel ParseOllamaResponse(string responseJson, CharacterDto character)
    {
        try
        {
            var message = JsonSerializer.Deserialize<OllamaResponseModel>(responseJson)?.Message;
            if (message is null)
            {
                _logger.LogError("Deserialized message is null for character ID '{Id}': {Response}", character.Id, responseJson);
                throw new InvalidOperationException($"Deserialized message is null for character ID '{character.Id}'.");
            }

            return message;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for character ID '{Id}': {Response}", character.Id, responseJson);
            throw new InvalidOperationException($"Failed to deserialize response for character ID '{character.Id}'.", ex);
        }
    }
}
