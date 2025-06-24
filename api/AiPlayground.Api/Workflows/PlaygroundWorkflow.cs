using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Workflows;

public class PlaygroundWorkflow
(
    ILogger<PlaygroundWorkflow> logger,
    PlaygroundRepository playgroundRepository
) : WorkflowBase(logger)
{
    private readonly PlaygroundRepository _playgroundRepository = playgroundRepository ?? throw new ArgumentNullException(nameof(playgroundRepository));

    public Task UpdateIterationsAsync()
    {
        return _playgroundRepository.UpdateIterationsAsync();
    }
}
