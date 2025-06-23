using AiPlayground.Api.Constants;
using AiPlayground.Api.Models.Playground;
using AiPlayground.Api.Workflows;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Api.Services;

public class PlaygroundService(
    CharacterWorkflow characterWorkflow,
    ConnectionWorkflow connectionWorkflow
)
{
    private readonly CharacterWorkflow _characterWorkflow = characterWorkflow ?? throw new ArgumentNullException(nameof(characterWorkflow));
    private readonly ConnectionWorkflow _connectionWorkflow = connectionWorkflow ?? throw new ArgumentNullException(nameof(connectionWorkflow));

    public async Task<CharacterDto> CreateCharacter(CreateCharacterInputModel characterInput)
    {
        var connection = _connectionWorkflow.CreateConnection(characterInput.Model);
        var character = await _characterWorkflow.CreateCharacterAsync(characterInput.Colour, connection, characterInput.GridPosition);
        
        return character is null 
            ? throw new InvalidOperationException("Failed to create character.") 
            : character;
    }

    public async Task<PlaygroundSetupResponseModel> GetSetup()
    {
        var model = new PlaygroundSetupResponseModel
        {
            AvailableModels = _characterWorkflow.GetAvailableModels(),
            Characters = await _characterWorkflow.GetCharactersAsync(),
            CellSize = PlaygroundConstants.DefaultCellSizePixels,
            GridWidth = PlaygroundConstants.DefaultGridSize,
            GridHeight = PlaygroundConstants.DefaultGridSize
        };

        return model;
    }
}
