using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Workflows;

public class PlaygroundWorkflow
(
    ILogger<PlaygroundWorkflow> logger,
    CharacterRepository characterRepository,
    PlaygroundRepository playgroundRepository
) : WorkflowBase(logger)
{
    private readonly ILogger<PlaygroundWorkflow> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly CharacterRepository _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
    private readonly PlaygroundRepository _playgroundRepository = playgroundRepository ?? throw new ArgumentNullException(nameof(playgroundRepository));

    public async Task<PlaygroundDto> GetPlaygroundAsync()
    {
        var playground = await _playgroundRepository.GetPlaygroundAsync();
        return playground.AsDto();
    }

    public async Task<PlaygroundDto> UpdateIterationsAsync()
    {
        var playground = await _playgroundRepository.UpdateIterationsAsync();
        return playground.AsDto();
    }

    public async Task ResetPlaygroundAsync()
    {
        await _characterRepository.ResetCharactersAsync();
        await _playgroundRepository.ResetPlaygroundAsync();
    }
}
