using AiPlayground.Api.ViewModels;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.Models.Playground;

namespace AiPlayground.Api.Services;

public class CharacterService(
    CharacterWorkflow characterWorkflow,
    ConnectionWorkflow connectionWorkflow
)
{
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));
    private readonly ConnectionWorkflow _connectionWorkflow = connectionWorkflow ?? throw new ArgumentNullException(nameof(connectionWorkflow));

    public async Task<CharacterViewModel> CreateCharacterAsync(CreateCharacterInputModel characterInput)
    {
        var connection = _connectionWorkflow.CreateConnection(characterInput.Model);
        var character = await _characterWorkflow.CreateCharacterAsync(characterInput.Colour, connection, characterInput.GridPosition);

        return character is null
            ? throw new InvalidOperationException("Failed to create character.")
            : new CharacterViewModel(character);
    }
}