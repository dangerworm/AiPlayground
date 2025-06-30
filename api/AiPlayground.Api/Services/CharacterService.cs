using AiPlayground.Api.ViewModels;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.Models.Playground;

namespace AiPlayground.Api.Services;

public class CharacterService(
    CharacterWorkflow characterWorkflow
)
{
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));

    public async Task<CharacterViewModel> CreateCharacterAsync(CreateCharacterInputModel characterInput)
    {
        var character = await _characterWorkflow.CreateCharacterAsync(characterInput.Colour, characterInput.GridPosition);

        return character is null
            ? throw new InvalidOperationException("Failed to create character.")
            : new CharacterViewModel(character);
    }
}