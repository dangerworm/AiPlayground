using AiPlayground.Api.Models.Playground;
using AiPlayground.Api.ViewModels;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.Constants;

namespace AiPlayground.Api.Services;

public class PlaygroundService(
    CharacterWorkflow characterWorkflow,
    ConnectionWorkflow connectionWorkflow,
    PlaygroundWorkflow playgroundWorkflow
)
{
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));
    private readonly ConnectionWorkflow _connectionWorkflow = connectionWorkflow ?? throw new ArgumentNullException(nameof(connectionWorkflow));
    private readonly PlaygroundWorkflow _playgroundWorkflow = playgroundWorkflow ?? throw new ArgumentNullException(nameof(playgroundWorkflow));

    public async Task<CharacterViewModel> CreateCharacterAsync(CreateCharacterInputModel characterInput)
    {
        var connection = _connectionWorkflow.CreateConnection(characterInput.Model);
        var character = await _characterWorkflow.CreateCharacterAsync(characterInput.Colour, connection, characterInput.GridPosition);
        
        return character is null 
            ? throw new InvalidOperationException("Failed to create character.") 
            : new CharacterViewModel(character);
    }

    public async Task<PlaygroundSetupResponseViewModel> GetSetupAsync()
    {
        var characters = await _characterWorkflow.GetCharactersAsync();
        
        var model = new PlaygroundSetupResponseViewModel
        {
            AvailableModels = _characterWorkflow.GetAvailableModels().ToList(),
            Characters = characters.Select(c => new CharacterViewModel(c)).ToList(),
            CellSize = PlaygroundConstants.DefaultCellSizePixels,
            GridWidth = PlaygroundConstants.DefaultGridSize,
            GridHeight = PlaygroundConstants.DefaultGridSize
        };

        return model;
    }

    public async Task UpdatePlaygroundIterationsAsync()
    {
        await _playgroundWorkflow.UpdateIterationsAsync();
    }
}
