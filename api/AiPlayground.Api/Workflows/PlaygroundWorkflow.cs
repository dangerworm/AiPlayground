using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Workflows;

public class PlaygroundWorkflow
(
    ILogger<PlaygroundWorkflow> logger,
    PlaygroundRepository playgroundRepository
) : WorkflowBase(logger)
{
    private readonly PlaygroundRepository _playgroundRepository = playgroundRepository ?? throw new ArgumentNullException(nameof(playgroundRepository));

    public async Task<PlaygroundDto> AddCharacterQuestion(Guid characterId, string question)
    {
        var playground = await _playgroundRepository.AddCharacterQuestion(characterId, question);
        return playground.AsDto();
    }

    public async Task<PlaygroundDto> GetPlaygroundAsync()
    {
        var playground = await _playgroundRepository.GetPlaygroundAsync();
        return playground.AsDto();
    }

    public Task UpdateIterationsAsync()
    {
        return _playgroundRepository.UpdateIterationsAsync();
    }
}
