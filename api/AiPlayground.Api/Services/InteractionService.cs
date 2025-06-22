using AiPlayground.Api.Models.Conversations;
using AiPlayground.Api.Models.Interactions;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Api.Services;

public class InteractionService(
    IHttpClientFactory httpClientFactory,
    ILogger<InteractionService> logger,
    CharacterWorkflow characterWorkflow,
    EnvironmentWorkflow environmentWorkflow,
    PromptWorkflow promptWorkflow
)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly ILogger<InteractionService> _logger = logger;
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));
    private readonly EnvironmentWorkflow _environmentWorkflow = environmentWorkflow ?? throw new ArgumentNullException(nameof(environmentWorkflow));
    private readonly PromptWorkflow _promptWorkflow = promptWorkflow ?? throw new ArgumentNullException(nameof(promptWorkflow));

    public async Task<string> ContactLlm(InteractInputModel characterInput)
    {
        var character = await _characterWorkflow.GetCharacterByIdAsync(characterInput.CharacterId);
        if (character is null)
        {
            _logger.LogError("Character with ID '{Id}' not found.", characterInput.CharacterId);
            throw new ArgumentException($"Character with ID '{characterInput.CharacterId}' not found.");
        }

        var chatModel = await BuildChatModel(character);
        var client = BuildClient(character.Connection);
        
        var response = await client.PostAsJsonAsync(character.Connection.Endpoint, chatModel);
        
        _logger.LogInformation("Generated output character with ID '{Id}': {Response}", character.Id, response);
        
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<ChatModel> BuildChatModel(CharacterDto character)
    {
        var systemPrompt = _promptWorkflow.GetSystemPrompt();
        var output = await _environmentWorkflow.BuildEnvironmentOutput(character.Id);

        var messages = new Dictionary<string, MessageModel>
        {
            { "system", new MessageModel { Role = "system", Content = systemPrompt } },
            { "user", new MessageModel { Role = "user", Content = output } }
        };

        return new ChatModel
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
}
